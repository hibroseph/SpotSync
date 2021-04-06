import React, { useState, useEffect } from "react";
import styled from "styled-components";
import Track from "./Track";
import Loader from "../../shared/Loader";
import CenteredHorizontally from "../../shared/CenteredHorizontally";
import { connect } from "react-redux";
import { getSpotifySearchResults, getUser, getPartyCode, getRealtimeConnection, getQueue } from "../../../redux/reducers/reducers";
import SearchInput from "./SearchInput";
import Subtitle from "../../shared/Subtitle";

const $SearchResults = styled.div`
  display: flex;
  flex-wrap: wrap;
`;

const Search = ({ search_results, user, partyCode, queue, connection }) => {
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    setIsLoading(false);
  }, search_results);

  return (
    <React.Fragment>
      <SearchInput setIsLoading={setIsLoading} placeholder="Search for Songs, Artists, and Albums" />
      {isLoading && (
        <CenteredHorizontally>
          <Loader></Loader>
        </CenteredHorizontally>
      )}
      {search_results?.length > 0 && (
        <React.Fragment>
          <Subtitle>Search Results</Subtitle>
          <$SearchResults>
            {search_results?.map((track) => {
              return (
                <Track
                  isValidAddition={() => {
                    console.log("checking to see if this is valid " + !queue?.some((song) => song.uri == track.uri));
                    return !queue?.some((song) => song.uri == track.uri);
                  }}
                  track={track}
                  partyCode={partyCode}
                  user={user}
                  connection={connection}
                ></Track>
              );
            })}
          </$SearchResults>
        </React.Fragment>
      )}
    </React.Fragment>
  );
};

const mapStateToProps = (state) => {
  return {
    search_results: getSpotifySearchResults(state),
    user: getUser(state),
    partyCode: getPartyCode(state),
    queue: getQueue(state),
    connection: getRealtimeConnection(state)?.connection,
  };
};

export default connect(mapStateToProps, null)(Search);
