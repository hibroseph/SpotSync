import { updateUserDetails } from "../redux/actions/user";

export const fetchUserDetails = () => {
  return (dispatch) => {
    fetch("/api/user/getUserDetails")
      .then((res) => res.json())
      .then((json) => {
        console.log(json);
        dispatch(updateUserDetails(json.isInParty, json.party, json.userDetails));
      });
  };
};
