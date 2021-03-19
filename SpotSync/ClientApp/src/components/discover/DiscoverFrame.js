import React from "react";
import styled from "styled-components";
import Header from "../shared/Header";
import SearchInput from "./SearchInput";
import { connect } from "react-redux";
import { getSpotifySearchResults, getUser, getPartyCode, getRealtimeConnection } from "../../redux/reducers/reducers";
import Track from "./Search/Track";

const $DiscoverFrame = styled.div``;
const $SearchResults = styled.div`
  display: flex;
  flex-wrap: wrap;
`;

const DiscoverFrame = (props) => {
  console.log("discoverFrame");
  console.log(props);
  return (
    <$DiscoverFrame className={props.className}>
      <Header>Discover</Header>
      <SearchInput placeholder="Search for Songs, Artists, and Albums" />
      <$SearchResults>
        {props?.search_results?.map((track) => {
          return <Track track={track} partyCode={props.partyCode} user={props.user} connection={props.connection}></Track>;
        })}
      </$SearchResults>
    </$DiscoverFrame>
  );
};

const mapStateToProps = (state) => {
  return {
    search_results: getSpotifySearchResults(state),
    user: getUser(state),
    partyCode: getPartyCode(state),
    connection: getRealtimeConnection(state)?.connection,
  };
};

export default connect(mapStateToProps, null)(DiscoverFrame);
