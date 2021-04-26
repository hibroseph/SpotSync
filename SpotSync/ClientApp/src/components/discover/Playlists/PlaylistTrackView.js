import React from "react";
import styled from "styled-components";
import TrackList from "../TrackList";
import Image from "../../shared/Image";
import Text from "../../shared/Text";

const $Image = styled(Image)``;

const $PlaylistTrackViewHeader = styled.div`
  padding: 5px 20px;
  margin-left: 20px;
`;

export default ({ playlistTracks, addToQueue }) => {
  return (
    <div>
      <$PlaylistTrackViewHeader>
        <$Image src={playlistTracks.playlist.playlistCoverArtUrl} />
        <Text>{playlistTracks.playlist.name}</Text>
      </$PlaylistTrackViewHeader>
      <TrackList tracks={playlistTracks.tracks.tracks} addToQueue={addToQueue}></TrackList>
    </div>
  );
};
