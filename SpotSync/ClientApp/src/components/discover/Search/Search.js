import React, { useState, useEffect } from "react";
import styled from "styled-components";
import Track from "./Track";
import Loader from "../../shared/Loader";
import CenteredHorizontally from "../../shared/CenteredHorizontally";
import { connect } from "react-redux";
import { getUser, getPartyCode, getRealtimeConnection, getQueue } from "../../../redux/reducers/reducers";
import SearchInput from "./SearchInput";
import Subtitle from "../../shared/Subtitle";

const $SearchResults = styled.div`
  display: flex;
  flex-wrap: wrap;
`;

const $SearchContainer = styled.div`
  border: 3px solid #fafafa;
  padding: 0 10px 10px;
  border-radius: 10px;
`;

const Search = ({ user, partyCode, queue, connection }) => {
  const [isLoading, setIsLoading] = useState(false);

  const [searchResults, setSearchResults] = useState([]);

  useEffect(() => {
    setIsLoading(false);
  }, [searchResults]);

  return (
    <$SearchContainer>
      <Subtitle>Search</Subtitle>
      <SearchInput setSearchResults={setSearchResults} setIsLoading={setIsLoading} placeholder="Search for Songs, Artists, and Albums" />
      {isLoading && (
        <CenteredHorizontally>
          <Loader></Loader>
        </CenteredHorizontally>
      )}
      {!isLoading && searchResults?.length > 0 && (
        <React.Fragment>
          <$SearchResults>
            {searchResults.map((track) => {
              return (
                <Track
                  isValidAddition={() => {
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
    </$SearchContainer>
  );
};

const mapStateToProps = (state) => {
  return {
    user: getUser(state),
    partyCode: getPartyCode(state),
    queue: getQueue(state),
    connection: getRealtimeConnection(state)?.connection,
  };
};

export default connect(mapStateToProps, null)(Search);
