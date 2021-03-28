import { checkingAuthentication, isAuthenticated, isNotAuthenticated } from "../redux/actions/authentication";

export const checkIfAuthenticated = () => {
  return (dispatch) => {
    dispatch(checkingAuthentication());

    fetch("/account/isauthenticated")
      .then((res) => {
        console.log("response from is authenticated");
        console.log(res);
        if (res.status == 200) {
          return res.json();
        }
      })
      .then((json) => {
        dispatch(isAuthenticated(json.userName));
      })
      .catch((res) => {
        console.log("opps");
        console.log(res);
        // TODO: error handling
        //window.location = "/account/login";
      });
  };
};
