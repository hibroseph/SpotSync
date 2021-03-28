import { updateUserDetails } from "../redux/actions/user";
import { connectToParty } from "./partyHub";
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
  console.log("getting user access token");
  fetch("/api/user/GetPartyGoerSpotifyAccessToken")
    .then((json) => json.json())
    .then((res) => dispatch(setUserAccessToken(res.accessToken)));
};
