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

const Search = ({ search_results, user, partyCode, queue, connection }) => {
  const [isLoading, setIsLoading] = useState(false);

  const [searchResults, setSearchResults] = useState([]);
  const [searchTerm, setSearchTerm] = useState("");

  useEffect(() => {
    setIsLoading(false);
  }, [searchResults]);

  return (
    <React.Fragment>
      <SearchInput
        setSearchTerm={setSearchTerm}
        setSearchResults={setSearchResults}
        setIsLoading={setIsLoading}
        placeholder="Search for Songs, Artists, and Albums"
      />
      {isLoading && (
        <CenteredHorizontally>
          <Loader></Loader>
        </CenteredHorizontally>
      )}
      {!isLoading && searchResults?.length > 0 && (
        <React.Fragment>
          <Subtitle>Search Results for {searchTerm}</Subtitle>
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
    </React.Fragment>
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
