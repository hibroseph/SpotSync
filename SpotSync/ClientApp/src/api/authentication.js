import { checkingAuthentication, isAuthenticated, isNotAuthenticated } from "../redux/actions/authentication";

export const checkIfAuthenticated = () => {
  console.log("inside checkIfAuthenticated");
  return (dispatch) => {
    console.log("inside dispatch of checkIfAuthenticated");
    dispatch(checkingAuthentication());

    fetch("/account/isauthenticated")
      .then((res) => {
        if (res.status == 200) {
          return res.json();
        }
      })
      .then((json) => {
        console.log("json from authentication");
        console.log(json);
        dispatch(isAuthenticated(json.userName));
      })
      .catch((res) => {
        console.log("response from isauthenticated in catch");
        console.log("redirecting to /account/login");
        //window.location = "/account/login";
      });
  };
};
