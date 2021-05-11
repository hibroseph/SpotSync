import { partyLeft, togglePlayback } from "../redux/actions/party";
import { connectToParty, userAddSongToQueue } from "./partyHub";

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

export const addContributionsToParty = (partyCode, contributions, contributionsToRemove) => {
  console.log("contributions", contributions);
  console.log("contributionsToRemove", contributionsToRemove);
  return fetch(`/api/party/updateContribution?partyCode=${partyCode}`, {
    method: "POST",
    body: JSON.stringify({ newContributions: contributions, contributionsToRemove: contributionsToRemove }),
    headers: {
      "Content-Type": "application/json",
    },
  });
};

export const getContributions = (partyCode) => {
  return fetch(`/api/party/usercontributions?partyCode=${partyCode}`).then((json) => json.json());
};

export const removeContribution = (partyCode, contributionIdToRemove) => {
  return fetch(`/api/party/removecontribution?partyCode=${partyCode}&contributionId=${contributionIdToRemove}`, {
    method: "DELETE",
  });
};
