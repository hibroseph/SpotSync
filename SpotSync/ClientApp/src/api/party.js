import { partyJoined, togglePlayback } from "../redux/actions/party";
import { connectToParty } from "./partyHub";
import { saveSpotifySearchResults } from "../redux/actions/party";
import { userAddSongToQueue } from "./partyHub";

export const createParty = () => {
  console.log("creating party");
  return (dispatch) => {
    fetch("/party/StartParty", {
      method: "POST",
    })
      .then((res) => res.json())
      .then((json) => {
        console.log(json);
        connectToParty(json.partyCode);
        dispatch(partyJoined(json.partyCode));
      });
  };
};

export const generateQueue = (partyCode) => {
  console.log("generating queue");

  fetch(`/party/UpdateQueueForParty?partyCode=${partyCode}`, { method: "POST" })
    .then((response) => {})
    .catch((error) => {
      console.error("SOMETHING HAPPENED THAT WAS AN ERROR");
      console.error(error);
    });
};

export const searchSpotify = (query, dispatch) => {
  fetch(`/api/user/searchSpotify?query=${query}&queryType=0`)
    .then((response) => response.json())
    .then((queryResults) => {
      console.log(queryResults);
      dispatch(saveSpotifySearchResults(queryResults));
    });
};

export const addSongToQueue = (song, user, partyCode, connection) => {
  userAddSongToQueue(song, user, partyCode, connection);
};

export const togglePlaybackState = (partyCode, dispatch) => {
  fetch(`/party/TogglePlaybackState?partyCode=${partyCode}`).then((res) => dispatch(togglePlayback()));
};
