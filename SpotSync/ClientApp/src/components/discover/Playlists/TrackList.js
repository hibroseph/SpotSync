import React from "react";
import styled from "styled-components";
import TrackListItem from "./TrackListItem";

const $TrackListContainer = styled.div`
  padding: 20px;
`;
export default ({ tracks, addToQueue }) => {
  return (
    <$TrackListContainer>
      {tracks.map((track, index) => {
        return <TrackListItem key={`${track.uri}_${index}`} track={track} index={index} addToQueue={() => addToQueue(track)} />;
      })}
    </$TrackListContainer>
  );
};
