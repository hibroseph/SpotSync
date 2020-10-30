import { LOG_IN, LOG_OUT, ADD_CREATE_PLAYLIST_SONGS } from "./ActionTypes";

export const IsUserAuthenticated = () => (dispatch) => {
  fetch("/api/account/isauthenticated").then((res) => {
    if (res.status == 200) {
      dispatch({ type: LOG_IN, payload: { loggedIn: true } });
    } else {
      dispatch({ type: LOG_OUT, payload: { loggedIn: false } });
    }
  });
};

export const LogInUser = () => (dispatch) => {
  fetch("/api/account/login").then((res) => {
    if (res.status == 200) {
      dispatch({ type: LOG_IN, payload: { loggedIn: true } });
    } else {
      dispatch({ type: LOG_OUT, payload: { loggedIn: false } });
    }
  });
};

export const GetUserSuggestedSongs = () => (dispatch) => {
  fetch("/api/user/SuggestedSongs").then((res) => {
    if (res.status == 200) {
      res.json().then((json) => {
        if (json != undefined) {
          dispatch({ type: ADD_CREATE_PLAYLIST_SONGS, payload: json });
        }
      });
    } else {
      // TODO: Add error messages to user
    }
  });
};
