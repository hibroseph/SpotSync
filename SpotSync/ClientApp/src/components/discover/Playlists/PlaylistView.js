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

export default ({ playlists, playlistTracks, addSomeTracksToQueue, viewPlaylist, isLoading, addToQueue }) => {
  const [playlistView, setPlaylistView] = useState("Playlists");
  return (
    <React.Fragment>
      {isLoading && (
        <CenteredHorizontally>
          <Loader></Loader>
        </CenteredHorizontally>
      )}
      {!isLoading && playlistView == "Playlists" && (
        <$PlaylistContainer>
          {playlists.map((playlist) => {
            return (
              <Playlist
                key={playlist.id}
                playlist={playlist}
                addSomeTracksToQueue={addSomeTracksToQueue}
                viewPlaylist={() => {
                  setPlaylistView("PlaylistTracks");
                  viewPlaylist(playlist);
                }}
              ></Playlist>
            );
          })}
        </$PlaylistContainer>
      )}

      {!isLoading && playlistView == "PlaylistTracks" && (
        <PlaylistTrackView playlistTracks={playlistTracks} addToQueue={addToQueue}></PlaylistTrackView>
      )}
    </React.Fragment>
  );
};
