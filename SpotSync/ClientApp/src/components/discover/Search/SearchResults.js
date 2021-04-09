import React from "react";
import Track from "./Track";
import Loader from "../../shared/Loader";
import CenteredHorizontally from "../../shared/CenteredHorizontally";
import styled from "styled-components";

const $SearchResults = styled.div`
  display: flex;
  flex-wrap: wrap;
`;
const SearchResults = ({ searchResults, isLoading, addSongToQueue }) => {
  return (
    <React.Fragment>
      {isLoading && (
        <CenteredHorizontally>
          <Loader></Loader>
        </CenteredHorizontally>
      )}
      {!isLoading && searchResults?.length > 0 && (
        <React.Fragment>
          <$SearchResults>
            {searchResults.map((track, index) => {
              return <Track key={`${track.uri}_${index}`} track={track} addSongToQueue={() => addSongToQueue(track)}></Track>;
            })}
          </$SearchResults>
        </React.Fragment>
      )}
    </React.Fragment>
  );
};

export default SearchResults;
