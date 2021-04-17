import React from "react";
import styled from "styled-components";
import TrackListItem from "./TrackListItem";

const $TrackListContainer = styled.div`
  padding: 5px 20px;
`;
export default ({ tracks, addToQueue }) => {
  return (
    <React.Fragment>
      <$TrackListContainer>
        {tracks?.map((track, index) => {
          return <TrackListItem key={`${track.uri}_${index}`} track={track} index={index + 1} addToQueue={() => addToQueue(track)} />;
        })}
      </$TrackListContainer>
    </React.Fragment>
  );
};
