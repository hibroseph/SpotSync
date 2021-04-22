using SpotSync.Domain.Contracts.SpotifyApi;
using SpotSync.Domain.Contracts.SpotifyApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using SpotibroModels = SpotSync.Domain.Contracts.SpotibroModels;
using SpotifyModels = SpotSync.Domain.Contracts.SpotifyApi.Models;

namespace SpotSync.Domain
{
    public class SpotifyToSpotibroModelMapper
    {
        public SpotibroModels.Track Convert(SpotifyModels.Track track)
        {
            return new SpotibroModels.Track
            {
                Album = new SpotibroModels.Album { Id = track.Album.Id, ImageUrl = track.Album.Images.FirstOrDefault()?.Url },
                Artists = track.Artists.Select(p => new SpotibroModels.Artist { Name = p.Name, Id = p.Id }).ToList(),
                Duration = track.Duration,
                Id = track.Id,
                IsExplicit = track.Explicit,
                Name = track.Name
            };
        }

        public List<SpotibroModels.Track> Convert(PagedObject<SpotifyModels.Track> tracks)
        {
            return tracks.Items.Select(p => Convert(p)).ToList();
        }

        public List<SpotibroModels.Track> Convert(List<SpotifyModels.Track> tracks)
        {
            return tracks.Select(p => Convert(p)).ToList();
        }

        public SpotibroModels.ArtistInformation Convert(SpotifyModels.ArtistInformation artistInformation)
        {
            return new SpotibroModels.ArtistInformation
            {
                Artist = new SpotibroModels.Artist { Id = artistInformation.Id, Name = artistInformation.Name },
            };
        }

        public SpotibroModels.Album Convert(SpotifyModels.Album album)
        {
            return new SpotibroModels.Album
            {
                Id = album.Id,
                ImageUrl = album.Images.FirstOrDefault().Url,
                Name = album.Name
            };
        }

        public SpotibroModels.Artist Convert(SpotifyModels.Artist artist)
        {
            return new SpotibroModels.Artist
            {
                Id = artist.Id,
                Name = artist.Name
            };
        }

        public List<SpotibroModels.Artist> Convert(List<SpotifyModels.Artist> artists)
        {
            return artists.Select(p => new SpotibroModels.Artist { Id = p.Id, Name = p.Name }).ToList();
        }

        public List<SpotibroModels.Track> Convert(TopTracks topTracks)
        {
            return topTracks.Tracks.Select(p => new SpotibroModels.Track
            {
                Name = p.Name,
                Album = Convert(p.Album),
                Artists = Convert(p.Artists),
                Duration = p.Duration,
                Id = p.Id,
                IsExplicit = p.Explicit,
            }).ToList();
        }

        public List<SpotibroModels.Image> Convert(List<SpotifyModels.Image> images)
        {
            return images?.Select(p => Convert(p)).ToList();
        }

        public SpotibroModels.Image Convert(SpotifyModels.Image image)
        {
            return new SpotibroModels.Image
            {
                Height = image.Height,
                Width = image.Width,
                Url = image.Url
            };
        }

        public SpotibroModels.Artist ConvertArtist(SpotifyModels.ArtistInformation artist)
        {
            return new SpotibroModels.Artist
            {
                Id = artist.Id,
                Name = artist.Name,
                Images = Convert(artist.Images)
            };
        }

        public SpotibroModels.ArtistInformation Convert(SpotifyModels.ArtistInformation artist, TopTracks topTracks)
        {
            return new SpotibroModels.ArtistInformation
            {
                Artist = ConvertArtist(artist),
                TopTracks = Convert(topTracks)
            };
        }
    }
}

