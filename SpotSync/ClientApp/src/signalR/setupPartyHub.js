import { setupSignalRConnection } from "./setupSignalRConnection";

import { useDispatch } from "react-redux";

const connectionHub = "/partyhub";
const getAccessToken = (state) => {
  return state.user.accessToken;
};
export const setupPartyHub = (dispatch) => setupSignalRConnection(connectionHub, {}, getAccessToken)(dispatch);
