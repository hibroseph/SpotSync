import { partyJoined, partyLeft, togglePlayback } from "../redux/actions/party";
import { connectToParty, userAddSongToQueue } from "./partyHub";
import { saveSpotifySearchResults } from "../redux/actions/party";

export const leaveParty = (partyCode) => {
  console.log("leaving party");
  return (dispatch) => {
    fetch(`/party/leaveparty?partyCode=${partyCode}`).then((res) => dispatch(partyLeft()));
  };
};

export const joinParty = (partyCode, connection) => {
  return fetch(`/party/joinparty?partyCode=${partyCode}`)
    .then((res) => res.json())
    .then((json) => {
      console.log(json);
      if (json.succeeded == true) {
        connectToParty(partyCode, connection);
        return { succeeded: true };
      } else {
        if (json.message != undefined) {
          return { succeeded: false, message: json.message };
        }
      }
    });
};

export const createParty = (connection) => {
  console.log("creating party");
  console.log(connection);
  return (dispatch) => {
    fetch("/party/StartParty", {
      method: "POST",
    })
      .then((res) => res.json())
      .then((json) => {
        console.log(json);
        connectToParty(json.partyCode, connection);
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
