import { LOG_IN, LOG_OUT } from "../ActionTypes";

const initialState = {
  loggedIn: false,
};

export default (state = initialState, action) => {
  switch (action.type) {
    case LOG_IN: {
      return { ...state, loggedIn: action.payload.loggedIn };
    }
    case LOG_OUT: {
      return { ...state, loggedIn: action.payload.loggedIn };
    }
    default:
      return state;
  }
};
