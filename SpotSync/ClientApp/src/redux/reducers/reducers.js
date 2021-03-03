const initalState = { user: { authentication: UNAUTHENTICATED } };

import { IS_AUTHENTICATED, CHECKING_AUTHENTICATION } from "../actions/authentication";
import { UPDATE_USER_DETAILS } from "../actions/user";
import { AUTHENTICATED, AUTHENTICATION_PENDING, UNAUTHENTICATED } from "../../states/authentication";

export default (state = initalState, action) => {
  switch (action.type) {
    case CHECKING_AUTHENTICATION: {
      console.log("checking authenticated in reduer");
      const newState = Object.assign({}, state, { user: { authentication: AUTHENTICATION_PENDING } });
      console.log(newState);
      return newState;
    }

    case IS_AUTHENTICATED: {
      console.log("the user is authenticated in reducer");

      return Object.assign({}, state, { user: { authentication: AUTHENTICATED, userName: action.userName } });
    }

    case UPDATE_USER_DETAILS: {
      console.log("updating user details");

      return Object.assign({}, state, state.user, {
        user: { details: { isInParty: action.isInParty, party: action.party, userDetails: action.userDetails } },
      });
    }
  }
  return state;
};

export const getAuthentication = (state) => state.user.authentication;
export const getUser = (state) => state.user;
