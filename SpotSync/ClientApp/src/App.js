import React, { useEffect, useState } from "react";
import Navigation from "./components/navigation/Navigation";
import MainContent from "./components/main/MainContent";
import connectToPartyHub, { setupPartyHub } from "./signalR/setupPartyHub";
import { checkIfAuthenticated } from "./api/authentication";
import { fetchUserDetails, getUserAccessToken } from "./api/user";
import { connect } from "react-redux";
import { getRealtimeConnection, getUser, getPartyCode } from "./redux/reducers/reducers";
import JoinOrCreateParty from "./components/main/JoinOrCreateParty";
import { setUpPartyHubApi } from "./api/partyHub";
import { connectToParty } from "./api/partyHub";
import { setUpSpotifyWebPlayback } from "./api/spotify";

const setUpProcess = (dispatch) => {
  checkIfAuthenticated()(dispatch);
  fetchUserDetails()(dispatch);
  getUserAccessToken(dispatch);
  setupPartyHub(dispatch);
  //connectToPartyHub();
};

const addSpotifyPlaybackScriptToDom = () => {
  console.log("adding script to dom");
  const script = document.createElement("script");

  script.src = "https://sdk.scdn.co/spotify-player.js";
  script.async = true;

  document.body.appendChild(script);
};

const checkToSeeIfUserIsInParty = (props) => {
  console.log("checking to see if user is in party");
  console.log(props);
  if (props?.isUserInParty && props.realTimeConnection?.connection && props?.accessToken) {
    console.log("connecting to party");
    setUpSpotifyWebPlayback(props.accessToken, props.realTimeConnection.connection);
    addSpotifyPlaybackScriptToDom();
    connectToParty(props.partyCode, props.realTimeConnection.connection);
  }
};

function App(props) {
  useEffect(() => {
    setUpProcess(props.dispatch);
  }, []);

  useEffect(() => {
    console.log("connection state or is in party changed");

    checkToSeeIfUserIsInParty(props);
  }, [props?.isUserInParty, props?.realTimeConnection?.connection]);

  return (
    <React.Fragment>
      {!props?.isUserInParty && <JoinOrCreateParty></JoinOrCreateParty>}
      <Navigation />
      <MainContent></MainContent>
    </React.Fragment>
  );
}

const mapStateToProps = (state) => {
  return {
    isUserInParty: getUser(state)?.isInParty,
    realTimeConnection: getRealtimeConnection(state),
    partyCode: getPartyCode(state),
    accessToken: getUser(state)?.accessToken,
  };
};

export default connect(mapStateToProps, null)(App);
