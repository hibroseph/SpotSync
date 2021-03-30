const initalState = { user: { authentication: UNAUTHENTICATED }, party: { queue: [], history: [] } };

import { IS_AUTHENTICATED, CHECKING_AUTHENTICATION } from "../actions/authentication";
import { UPDATE_USER_DETAILS, UPDATE_USER_ACCESS_TOKEN } from "../actions/user";
import { AUTHENTICATED, AUTHENTICATION_PENDING, UNAUTHENTICATED } from "../../states/authentication";
import { CONNECTED, DISCONNECTED } from "../../states/signalr";
import {
  PARTY_JOINED,
  LEFT_PARTY,
  UPDATE_HISTORY,
  UPDATE_QUEUE,
  SEARCHED_SPOTIFY,
  TOGGLE_PLAYBACK,
  UPDATE_SONG,
  UPDATE_CURRENT_SONG,
  USER_LIKES_SONG,
  USER_DISLIKES_SONG,
  SET_SONG_FEELINGS,
} from "../actions/party";
import { REALTIME_CONNECTION_ESTABLISHED } from "../actions/signalr";

export default (state = initalState, action) => {
  switch (action.type) {
    case SET_SONG_FEELINGS: {
      return Object.assign({}, state, {
        party: Object.assign({}, state.party, { songFeelings: action.songFeelings }),
      });
    }

    case USER_LIKES_SONG: {
      return Object.assign({}, state, {
        party: Object.assign({}, state.party, { songFeelings: Object.assign({}, state.party.songFeelings, { [action.trackUri]: 1 }) }),
      });
    }

    case USER_DISLIKES_SONG: {
      return Object.assign({}, state, {
        party: Object.assign({}, state.party, { songFeelings: Object.assign({}, state.party.songFeelings, { [action.trackUri]: 0 }) }),
      });
    }

    case UPDATE_CURRENT_SONG: {
      return Object.assign({}, state, { nowPlaying: action.track });
    }

    case LEFT_PARTY: {
      return Object.assign(
        {},
        state,
        {
          user: Object.assign({}, state.user, { details: Object.assign({}, state.user.details, { isInParty: false }) }),
        },
        { party: { queue: [], history: [] } }
      );
    }

    case UPDATE_SONG: {
      // move song in now playing to history
      // remove song that came in action from queue
      // move song to now playing
      /*
      return Object.assign({}, state, {
        party: Object.assign({}, state.party, { queue: state.party.queue.map((song) => song.uri != action.song.uri) }),
      });
      */
      console.log("updating song");

      let indexOfSongToRemove = state.party.queue.findIndex((song) => song.uri == action.song.uri);

      console.log("WHAT DOES HISTORY EQUAL?");
      console.log(state.party.nowPlaying ? "hello" : "goodbye");

      console.log("index of the song to remove " + indexOfSongToRemove);
      return Object.assign({}, state, {
        party: Object.assign(
          {},
          state.party,
          { nowPlaying: action.song },
          {
            queue:
              indexOfSongToRemove == -1
                ? state.party.queue
                : [...state.party.queue.slice(0, indexOfSongToRemove), ...state.party.queue.slice(indexOfSongToRemove + 1)],
          },
          { history: state.party.nowPlaying ? [...state.party.history, state.party.nowPlaying] : state.party.history }
        ),
      });
    }

    case TOGGLE_PLAYBACK: {
      return Object.assign({}, state, { user: { details: Object.assign({}, state.user.details, { pausedMusic: !state.user.details.pausedMusic }) } });
    }

    case UPDATE_USER_ACCESS_TOKEN: {
      return Object.assign({}, state, { user: Object.assign({}, state.user, { accessToken: action.accessToken }) });
    }

    case SEARCHED_SPOTIFY: {
      return Object.assign({}, state, { search_results: action.results });
    }

    case UPDATE_QUEUE: {
      console.log("queue before updating");
      console.log(state.party.queue);
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
      console.log("updating is authenticated");
      return Object.assign({}, state, {
        user: Object.assign({}, state.user, { authentication: AUTHENTICATED }),
      });
    }

    case UPDATE_USER_DETAILS: {
      console.log("UPDATING USER DETAILS");
      return Object.assign(
        {},
        state,
        {
          user: Object.assign(
            {},
            {
              details: Object.assign({}, action.userDetails, { isInParty: action.isInParty }, state.user.details),
            },
            state.user
          ),
        },
        { party: Object.assign({}, { code: action.party.partyCode }, state.party) }
      );
    }

    case PARTY_JOINED: {
      console.log("PARTY JOINED");
      return Object.assign(
        {},
        state,
        {
          user: Object.assign({}, state.user, { details: Object.assign({}, state.user.details, { isInParty: true }) }),
        },
        { party: Object.assign({}, { code: action.partyCode }, state.party) }
      );
    }
  }
  return state;
};

export const getAuthentication = (state) => state.user.authentication;
export const getUser = (state) => state.user;
export const getPartyCode = (state) => state?.party?.code;
export const getSongFeelings = (state) => state?.party?.songFeelings;
export const getRealtimeConnection = (state) => {
  return {
    connectionState: state.connectionState,
    connection: state.connection,
  };
};
export const getCurrentSong = (state) => state?.party?.nowPlaying;
export const getParty = (state) => state.party;
export const getSpotifySearchResults = (state) => state.search_results;