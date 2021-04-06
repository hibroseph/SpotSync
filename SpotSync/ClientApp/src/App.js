import React, { useEffect, useState } from "react";
import Navigation from "./components/navigation/Navigation";
import MainContent from "./components/main/MainContent";
import { setupPartyHub } from "./signalR/setupPartyHub";
import { checkIfAuthenticated } from "./api/authentication";
import { fetchUserDetails, getUserAccessToken } from "./api/user";
import { connect } from "react-redux";
import { getRealtimeConnection, getUser, getPartyCode } from "./redux/reducers/reducers";
import JoinOrCreateParty from "./components/main/JoinOrCreateParty";
import { setUpSpotifyWebPlayback } from "./api/spotify";
import NowPlaying from "../src/components/main/NowPlaying/NowPlaying";

const setUpProcess = (dispatch) => {
  checkIfAuthenticated()(dispatch);
  fetchUserDetails()(dispatch);
  getUserAccessToken(dispatch);
  setupPartyHub(dispatch);
};

const addSpotifyPlaybackScriptToDom = () => {
  const script = document.createElement("script");

  script.src = "https://sdk.scdn.co/spotify-player.js";
  script.async = true;

  document.body.appendChild(script);
};

const checkToSeeIfUserIsInParty = (props) => {
  if (props?.isUserInParty && props.realTimeConnection?.connection && props?.accessToken) {
    setUpSpotifyWebPlayback(props.accessToken, props.realTimeConnection.connection);
    addSpotifyPlaybackScriptToDom();
  }
};

function App(props) {
  useEffect(() => {
    setUpProcess(props.dispatch);
  }, []);

  useEffect(() => {
    checkToSeeIfUserIsInParty(props);
  }, [props?.isUserInParty, props?.realTimeConnection?.connection]);

  return (
    <React.Fragment>
      {!props?.isUserInParty && <JoinOrCreateParty></JoinOrCreateParty>}
      <Navigation />
      <MainContent></MainContent>
      <NowPlaying></NowPlaying>
    </React.Fragment>
  );
}

const mapStateToProps = (state) => {
  return {
    isUserInParty: getUser(state)?.details?.isInParty,
    realTimeConnection: getRealtimeConnection(state),
    partyCode: getPartyCode(state),
    accessToken: getUser(state)?.accessToken,
  };
};

export default connect(mapStateToProps, null)(App);
