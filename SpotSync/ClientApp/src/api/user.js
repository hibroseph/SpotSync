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
