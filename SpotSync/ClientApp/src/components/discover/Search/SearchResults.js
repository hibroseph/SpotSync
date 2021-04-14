import React from "react";
import Track from "./Track";
import Loader from "../../shared/Loader";
import CenteredHorizontally from "../../shared/CenteredHorizontally";
import styled from "styled-components";

const $SearchResults = styled.div`
  display: flex;
  flex-wrap: wrap;
`;
const SearchResults = ({ searchTerm, searchResults, isLoading, addSongToQueue }) => {
  return (
    <React.Fragment>
      {isLoading && (
        <CenteredHorizontally>
          <Loader></Loader>
        </CenteredHorizontally>
      )}
      {!isLoading && searchResults?.length > 0 && (
        <React.Fragment>
          <p>Search Results for {searchTerm}</p>
          <$SearchResults>
            {searchResults.map((track, index) => {
              return <Track key={`${track.iD}_${index}`} track={track} addSongToQueue={() => addSongToQueue(track)}></Track>;
            })}
          </$SearchResults>
        </React.Fragment>
      )}
    </React.Fragment>
  );
};

export default SearchResults;
