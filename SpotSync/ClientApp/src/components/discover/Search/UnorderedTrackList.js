import React from "react";
import Track from "./Track";
import Loader from "../../shared/Loader";
import CenteredHorizontally from "../../shared/CenteredHorizontally";
import styled from "styled-components";

const UnorderedTrackListContainer = styled.div`
  display: flex;
  flex-wrap: wrap;
`;
const UnorderedTrackList = ({ tracks, isLoading, addSongToQueue }) => {
  return (
    <React.Fragment>
      {isLoading && (
        <CenteredHorizontally>
          <Loader></Loader>
        </CenteredHorizontally>
      )}
      {!isLoading && tracks?.length > 0 && (
        <React.Fragment>
          <UnorderedTrackListContainer>
            {tracks.map((track, index) => {
              return <Track key={`${track.id}_${index}`} track={track} addSongToQueue={() => addSongToQueue(track)}></Track>;
            })}
          </UnorderedTrackListContainer>
        </React.Fragment>
      )}
    </React.Fragment>
  );
};

export default UnorderedTrackList;
