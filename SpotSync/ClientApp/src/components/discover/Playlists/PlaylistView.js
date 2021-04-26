import React, { useState } from "react";
import styled from "styled-components";
import Playlist from "./Playlist";
import Loader from "../../shared/Loader";
import CenteredHorizontally from "../../shared/CenteredHorizontally";
import PlaylistTrackView from "./PlaylistTrackView";

const $PlaylistContainer = styled.div`
  display: flex;
  flex-wrap: wrap;
`;

const PLAYLIST_VIEW = "Playlists";
const PLAYLIST_TRACKS = "PlaylistsTracks";

export default ({ playlists, playlistTracks, addSomeTracksToQueue, viewPlaylist, isLoading, addToQueue }) => {
  const [playlistView, setPlaylistView] = useState(PLAYLIST_VIEW);
  return (
    <React.Fragment>
      {isLoading && (
        <CenteredHorizontally>
          <Loader></Loader>
        </CenteredHorizontally>
      )}
      {!isLoading && playlistView == PLAYLIST_VIEW && (
        <$PlaylistContainer>
          {playlists.map((playlist) => {
            return (
              <Playlist
                key={playlist.id}
                playlist={playlist}
                addSomeTracksToQueue={addSomeTracksToQueue}
                viewPlaylist={() => {
                  setPlaylistView(PLAYLIST_TRACKS);
                  viewPlaylist(playlist).catch((p) => {
                    setPlaylistView(PLAYLIST_VIEW);
                  });
                }}
              ></Playlist>
            );
          })}
        </$PlaylistContainer>
      )}

      {!isLoading && playlistView == PLAYLIST_TRACKS && (
        <PlaylistTrackView playlistTracks={playlistTracks} addToQueue={addToQueue}></PlaylistTrackView>
      )}
    </React.Fragment>
  );
};
