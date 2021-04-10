import React from "react";
import styled from "styled-components";
import TrackList from "./TrackList";
import Image from "../../shared/Image";
import Text from "../../shared/Text";

const $PlaylistTrackViewHeader = styled.div`
  padding: 5px 20px;
  margin-left: 20px;
`;

export default ({ playlistTracks, addToQueue }) => (
  <div>
    <$PlaylistTrackViewHeader>
      <Image src={playlistTracks.playlist.playlistImageUrl} />
      <Text>{playlistTracks.playlist.name}</Text>
    </$PlaylistTrackViewHeader>
    <TrackList tracks={playlistTracks.tracks} addToQueue={addToQueue}></TrackList>
  </div>
);
