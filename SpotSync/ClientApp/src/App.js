import React, { useEffect, useState } from "react";
import Navigation from "./components/navigation/Navigation";
import MainContent from "./components/main/MainContent";
import { setupPartyHub } from "./signalR/setupPartyHub";
import { checkIfAuthenticated } from "./api/authentication";
import { fetchUserDetails, getUserAccessToken } from "./api/user";
import { connect } from "react-redux";
import { getRealtimeConnection, getUser, getPartyCode } from "./redux/reducers/reducers";
import { setUpSpotifyWebPlayback } from "./api/spotify";
import NowPlaying from "./components/main/NowPlaying/NowPlaying";
import MusicContributionPopup from "./components/popups/MusicContributionPopup";
import JoinOrCreateParty from "./components/popups/JoinOrCreateParty";

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

const configureSpotifySdk = (props, spotifySdkAdded, setSpotifySdkAdded) => {
  if (props.realTimeConnection?.connection && !spotifySdkAdded) {
    setUpSpotifyWebPlayback(props.realTimeConnection.connection);
    addSpotifyPlaybackScriptToDom();
    setSpotifySdkAdded(true);
  }
};

const MUSIC_CONTRIBUTIONS = "Music Contributions";
const JOIN_OR_CREATE_PARTY = "Join or Create Party";

const Popups = (popupName, setPopup, partyCode, setPartyInitalized, setContributions) => {
  switch (popupName) {
    case JOIN_OR_CREATE_PARTY:
      return <JoinOrCreateParty showContributionsPopup={(show) => (show ? setPopup(MUSIC_CONTRIBUTIONS) : setPopup(null))}></JoinOrCreateParty>;
    case MUSIC_CONTRIBUTIONS:
      return (
        <MusicContributionPopup
          setPartyInitalized={setPartyInitalized}
          setGlobalContributions={setContributions}
          setPopup={setPopup}
          partyCode={partyCode}
          hideMusicContributionPopup={() => setPopup(null)}
        ></MusicContributionPopup>
      );
    default:
      return null;
  }
};
function App(props) {
  const [spotifySdkAdded, setSpotifySdkAdded] = useState(false);
  const [isPartyInitalized, setPartyInitalized] = useState(false);
  const [contributions, setContributions] = useState([]);

  useEffect(() => {
    setUpProcess(props.dispatch);
  }, []);

  useEffect(() => {
    configureSpotifySdk(props, spotifySdkAdded, setSpotifySdkAdded);
  }, [props?.isUserInParty, props?.realTimeConnection?.connection]);

  const [artistView, setArtistView] = useState();
  const [currentPopup, setCurrentPopup] = useState(JOIN_OR_CREATE_PARTY);

  return (
    <React.Fragment>
      {Popups(currentPopup, setCurrentPopup, props.partyCode, setPartyInitalized, setContributions)}
      <Navigation setPartyInitalized={setPartyInitalized} showCreateOrJoinPartyPopup={() => setCurrentPopup(JOIN_OR_CREATE_PARTY)} />
      <MainContent
        showContributionsPopup={() => setCurrentPopup(MUSIC_CONTRIBUTIONS)}
        contributions={contributions}
        artistView={artistView}
        setContributions={setContributions}
      ></MainContent>
      <NowPlaying ShowArtistView={(artist) => setArtistView(artist)}></NowPlaying>
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
