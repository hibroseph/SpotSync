const initalState = { user: { authentication: UNAUTHENTICATED }, party: { queue: [], history: [] } };

import { IS_AUTHENTICATED, CHECKING_AUTHENTICATION } from "../actions/authentication";
import { UPDATE_USER_DETAILS, UPDATE_USER_ACCESS_TOKEN } from "../actions/user";
import { AUTHENTICATED, AUTHENTICATION_PENDING, UNAUTHENTICATED } from "../../states/authentication";
import { CONNECTED, DISCONNECTED } from "../../states/signalr";
import { PARTY_JOINED, LEFT_PARTY, UPDATE_HISTORY, UPDATE_QUEUE, SEARCHED_SPOTIFY, TOGGLE_PLAYBACK } from "../actions/party";
import { REALTIME_CONNECTION_ESTABLISHED } from "../actions/signalr";

export default (state = initalState, action) => {
  switch (action.type) {
    case LEFT_PARTY: {
      return Object.assign(
        {},
        state,
        state.user,
        {
          user: { details: { isInParty: false } },
        },
        state.party,
        { party: null }
      );
    }

    case TOGGLE_PLAYBACK: {
      console.log("TOGGLE PLAYBACK");
      return Object.assign({}, state, { user: { details: Object.assign({}, state.user.details, { pausedMusic: !state.user.details.pausedMusic }) } });
    }

    case UPDATE_USER_ACCESS_TOKEN: {
      return Object.assign({}, state, { user: Object.assign({}, state.user, { accessToken: action.accessToken }) });
    }

    case SEARCHED_SPOTIFY: {
      return Object.assign({}, state, { search_results: action.results });
    }

    case UPDATE_QUEUE: {
      console.log("updating queue");
      console.log(action.queue);
      return Object.assign({}, state, { party: Object.assign({}, state.party, { queue: action.queue }) });
    }

    case UPDATE_HISTORY: {
      return Object.assign({}, state, { party: Object.assign({}, state.party, { history: action.history }) });
    }

    case REALTIME_CONNECTION_ESTABLISHED: {
      return Object.assign({}, state, { connectionState: CONNECTED, connection: action.connection });
    }

    case CHECKING_AUTHENTICATION: {
      return Object.assign({}, state, { user: { authentication: AUTHENTICATION_PENDING } });
    }

    case IS_AUTHENTICATED: {
      return Object.assign({}, state, state.user, {
        user: Object.assign({}, { authentication: AUTHENTICATED }, state.user.details),
      });
    }

    case UPDATE_USER_DETAILS: {
      return Object.assign(
        {},
        state,
        {
          user: {
            details: Object.assign({}, action.userDetails, { isInParty: action.isInParty }, state.user.details),
            authentication: state.user.authentication,
          },
        },
        state.party,
        { party: { code: action.party.partyCode } }
      );
    }

    case PARTY_JOINED: {
      return Object.assign(
        {},
        state,
        {
          user: { details: { isInParty: true } },
        },
        state.party,
        { party: { code: action.partyCode } }
      );
    }
  }
  return state;
};

export const getAuthentication = (state) => state.user.authentication;
export const getUser = (state) => state.user;
export const getPartyCode = (state) => state?.party?.code;
export const getRealtimeConnection = (state) => {
  return {
    connectionState: state.connectionState,
    connection: state.connection,
  };
};
export const getParty = (state) => state.party;
export const getSpotifySearchResults = (state) => state.search_results;
