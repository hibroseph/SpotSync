import { partyJoined, partyLeft, togglePlayback } from "../redux/actions/party";
import { connectToParty, userAddSongToQueue } from "./partyHub";
import { saveSpotifySearchResults } from "../redux/actions/party";

export const leaveParty = (partyCode) => {
  return (dispatch) => {
    fetch(`/party/leaveparty?partyCode=${partyCode}`).then((res) => dispatch(partyLeft()));
  };
};

export const joinParty = (partyCode, connection) => {
  return fetch(`/party/joinparty?partyCode=${partyCode}`)
    .then((res) => res.json())
    .then((json) => {
      if (json.succeeded == true) {
        console.log("connecting to party");
        connectToParty(partyCode, connection);
        return { succeeded: true };
      } else {
        console.log("failed to connect to party");
        if (json.message != undefined) {
          return { succeeded: false, message: json.message };
        }
      }
    });
};

export const createParty = (connection) => {
  return fetch("/party/StartParty", {
    method: "POST",
  })
    .then((res) => res.json())
    .then((json) => {
      return connectToParty(json.partyCode, connection);
    });
};

export const generateQueue = (partyCode) => {
  fetch(`/party/UpdateQueueForParty?partyCode=${partyCode}`, { method: "POST" })
    .then((response) => {})
    .catch((error) => {
      console.error("SOMETHING HAPPENED THAT WAS AN ERROR");
      console.error(error);
    });
};

export const searchSpotify = (query) => {
  return fetch(`/api/user/searchSpotify?query=${query}&queryType=0`)
    .then((response) => response.json())
    .then((searchResults) => {
      return searchResults;
    });
};

export const addSongToQueue = (song, user, partyCode, connection) => {
  userAddSongToQueue(song, user, partyCode, connection);
};

export const togglePlaybackState = (partyCode, dispatch) => {
  fetch(`/party/TogglePlaybackState?partyCode=${partyCode}`).then((res) => dispatch(togglePlayback()));
};

export const getUserLikesDislikes = (partyCode) => {
  return fetch(`/api/party/UsersLikesDislikes?partyCode=${partyCode}`)
    .then((res) => res.json())
    .then((json) => {
      if (json.succeeded) {
        return json.content;
      }
    });
};

export const getTopSongs = (amount) => {
  return fetch(`/api/party/SuggestedSongs?limit=${amount}`)
    .then((res) => res.json())
    .then((json) => json);
};
