using SpotSync.Domain.Contracts.SpotibroModels;
using SpotSync.Domain.Contracts.SpotifyApi.Models;
using SpotSync.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using SpotibroModels = SpotSync.Domain.Contracts.SpotibroModels;

namespace SpotSync.Domain
{
    public class Queue
    {
        private List<TrackWithFeelings> _tracks;
        private UsersLikesDislikes _usersLikesDislikes;
        private Random _random;
        public Queue(List<Track> tracks)
        {
            _tracks = tracks.Select(p => new TrackWithFeelings(p)).ToList();
            _tracks.Sort(new ReorderQueueComparer());
            _usersLikesDislikes = new UsersLikesDislikes();
            _random = new Random();
        }

        public Queue()
        {
            _tracks = new List<TrackWithFeelings>();
            _usersLikesDislikes = new UsersLikesDislikes();
            _random = new Random();
        }

        public Dictionary<string, int> GetTrackVoting()
        {
            return _usersLikesDislikes.GetTrackFeelings();
        }

        public bool SongsExistInQueue()
        {
            return _tracks.Count > 0;
        }
        public List<Track> GetAllTracks()
        {
            return _tracks.Select(p => p.GetTrackWithoutFeelings()).ToList();
        }

        public TrackWithFeelings NextTrack()
        {
            if (HasNextTrack())
            {
                TrackWithFeelings trackToReturn = _tracks.First();

                _tracks.RemoveAt(0);

                return trackToReturn;
            }

            return null;
        }

        public bool TracksAreLiked()
        {
            foreach (TrackWithFeelings track in _tracks)
            {
                if (track.DoUsersLikeSong())
                {
                    return true;
                }
            }

            return false;
        }

        public List<string> GetRandomLikedTrackUris(int amount)
        {
            return _tracks.GetRandomNItems(5).Select(track => track.GetTrackWithoutFeelings().Id).ToList();
        }

        public LikesDislikes GetUsersTrackFeelings(PartyGoer user)
        {
            return _usersLikesDislikes.GetUsersTrackFeelings(user);
        }

        public async Task UserLikesTrackAsync(PartyGoer user, string trackUri, string partyCode)
        {
            if (!_usersLikesDislikes.DoesUserLikeTrack(user, trackUri))
            {
                _usersLikesDislikes.UserLikesTrack(user, trackUri);

                TrackWithFeelings track = _tracks.Find(p => p.GetTrackWithoutFeelings().Id.Equals(trackUri, StringComparison.OrdinalIgnoreCase));

                if (track != null)
                {
                    track.UserLikesTrack();
                    _tracks.Sort(new ReorderQueueComparer());

                    await DomainEvents.RaiseAsync(new UpdateQueue { Tracks = _tracks.Select(p => p.GetTrackWithoutFeelings()).ToList(), PartyCode = partyCode });
                }
            }
        }

        public async Task UserDislikesTrackAsync(PartyGoer user, string trackUri, int listenerCount, string partyCode)
        {
            if (!_usersLikesDislikes.DoesUserDislikeTrack(user, trackUri))
            {
                _usersLikesDislikes.UserDislikesTrack(user, trackUri);

                TrackWithFeelings track = _tracks.Find(p => p.GetTrackWithoutFeelings().Id.Equals(trackUri, StringComparison.OrdinalIgnoreCase));

                if (track != null && track.DislikeCount() + 1 > listenerCount * 0.5)
                {
                    _tracks.RemoveAll(p => p.GetTrackWithoutFeelings().Id.Equals(trackUri, StringComparison.OrdinalIgnoreCase));

                    await DomainEvents.RaiseAsync(new UpdateQueue { Tracks = _tracks.Select(p => p.GetTrackWithoutFeelings()).ToList(), PartyCode = partyCode });
                }
            }
        }

        public List<string> GetLikedTrackUris()
        {
            List<string> likedTrackUris = new List<string>();

            foreach (TrackWithFeelings track in _tracks)
            {
                if (track.DoUsersLikeSong())
                {
                    likedTrackUris.Add(track.GetTrackWithoutFeelings().Id);
                }
            }

            return likedTrackUris;
        }

        public bool HasExplicitTracks()
        {
            return _tracks.Any(p => p.GetTrackWithoutFeelings().Explicit);
        }

        public bool HasNextTrack()
        {
            return _tracks.Any();
        }

        public bool SongExistsInQueue(Track track)
        {
            return _tracks.Any(p => p.GetTrackWithoutFeelings().Id.Equals(track.Id, StringComparison.OrdinalIgnoreCase));
        }
        public void QueueTrack(Track track)
        {
            _tracks.Add(new TrackWithFeelings(track));
        }

        public Task AddTracksRandomlyToQueueAsync(List<SpotibroModels.Track> tracks)
        {
            foreach (SpotibroModels.Track track in tracks)
            {
                _tracks.Insert(GetRandomQueueIndex(), new TrackWithFeelings(new Track
                {
                    AlbumImageUrl = track.Album.ImageUrl,
                    Artists = track.Artists.Select(p => new Contracts.SpotifyApi.Models.Artist { Id = p.Id, Name = p.Name }).ToList(),
                    Explicit = track.IsExplicit,
                    Id = track.Id,
                    Length = track.Duration,
                    Name = track.Name
                }
                ));
            }

            return Task.CompletedTask;
        }

        private int GetRandomQueueIndex()
        {
            return _random.Next(0, (_tracks.Count > 0 ? _tracks.Count - 1 : 0));
        }
    }
}
