import { updateUserDetails } from "../redux/actions/user";
import { setUserAccessToken } from "../redux/actions/user";
export const fetchUserDetails = () => {
  return (dispatch) => {
    fetch("/api/user/getUserDetails")
      .then((res) => res.json())
      .then((json) => {
        dispatch(updateUserDetails(json.isInParty, json.party, json.userDetails));
      });
  };
};

export const getUserAccessToken = (dispatch) => {
  fetch("/api/user/GetPartyGoerSpotifyAccessToken")
    .then((json) => json.json())
    .then((res) => dispatch(setUserAccessToken(res.accessToken)));
};

export const getTopSongs = (amount) => {
  return fetch(`/api/user/SuggestedSongs?limit=${amount}`)
    .then((res) => res.json())
    .then((json) => json);
};

export const getPlaylists = (limit, offset) => {
  return fetch(`/api/user/usersplaylists?limit=${limit}&offset=${offset}`)
    .then((response) => response.json())
    .then((json) => json);
};

export const getPlaylistItems = (playlistId) => {
  return fetch(`/api/user/usersplaylistitems?playlistId=${playlistId}`)
    .then((res) => res.json())
    .then((json) => json);
};

export const getFavoriteTracks = () => {
  return fetch("/api/user/getfavoritetracks")
    .then((res) => res.json())
    .then((json) => json);
};

export const favoriteTrack = (trackId) => {
  return fetch(`/api/user/favoritetrack?trackId=${trackId.split("+")[0]}`, {
    method: "POST",
  });
};

export const unfavoriteTrack = (trackId) => {
  return fetch(`/api/user/unfavoritetrack?trackId=${trackId.split("+")[0]}`, { method: "POST" });
};

export const getSuggestedContributions = (excludedIds) => {
  console.log("getting contributions");
  return fetch(`/api/user/suggestedcontributions`, {
    method: "POST",
    body: JSON.stringify(excludedIds),
    headers: {
      "Content-Type": "application/json",
    },
  }).then((res) => res.json());
};
