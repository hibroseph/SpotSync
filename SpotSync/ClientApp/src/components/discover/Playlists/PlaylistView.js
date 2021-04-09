import React from "react";
import styled from "styled-components";
import Playlist from "./Playlist";
import Loader from "../../shared/Loader";
import CenteredHorizontally from "../../shared/CenteredHorizontally";

const $PlaylistContainer = styled.div`
  display: flex;
  flex-wrap: wrap;
`;

export default ({ playlists, addSomeTracksToQueue, viewPlaylist, isLoading }) => {
  return (
    <React.Fragment>
      {isLoading && (
        <CenteredHorizontally>
          <Loader></Loader>
        </CenteredHorizontally>
      )}
      <$PlaylistContainer>
        {playlists.map((playlist) => {
          return <Playlist id={playlist.id} playlist={playlist} addSomeTracksToQueue={addSomeTracksToQueue} viewPlaylist={viewPlaylist}></Playlist>;
        })}
      </$PlaylistContainer>
    </React.Fragment>
  );
};
