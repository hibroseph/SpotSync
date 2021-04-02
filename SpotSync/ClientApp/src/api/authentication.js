import { checkingAuthentication, isAuthenticated, isNotAuthenticated } from "../redux/actions/authentication";

export const checkIfAuthenticated = () => {
  return (dispatch) => {
    dispatch(checkingAuthentication());

    fetch("/account/isauthenticated")
      .then((res) => {
        if (res.status == 200) {
          return res.json();
        }
      })
      .then((json) => {
        dispatch(isAuthenticated(json.userName));
      })
      .catch((res) => {
        // TODO: error handling
        //window.location = "/account/login";
      });
  };
};
