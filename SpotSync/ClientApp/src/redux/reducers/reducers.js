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
  TOGGLE_PLAYBACK,
  UPDATE_SONG,
  UPDATE_CURRENT_SONG,
  USER_LIKES_SONG,
  USER_DISLIKES_SONG,
  SET_SONG_FEELINGS,
  LISTENER_JOINED,
  LISTENER_LEFT,
  UPDATE_TRACK_VOTES,
  UPDATE_POSITION_IN_SONG,
} from "../actions/party";
import { SHOW_ARTIST_VIEW } from "../actions/views";

import { REALTIME_CONNECTION_ESTABLISHED } from "../actions/signalr";

export default (state = initalState, action) => {
  switch (action.type) {
    case UPDATE_POSITION_IN_SONG: {
      return Object.assign({}, state, {
        party: Object.assign({}, state.party, { nowPlaying: Object.assign({}, state.party.nowPlaying, { startPosition: action.position }) }),
      });
    }

    case UPDATE_TRACK_VOTES: {
      return Object.assign({}, state, { party: Object.assign({}, state.party, { trackVotes: action.trackVotes }) });
    }

    case SHOW_ARTIST_VIEW: {
      return Object.assign({}, state, { views: { searchArtistId: action.artist } });
    }

    case LISTENER_LEFT: {
      let indexOfListenerToRemove = state.party.listeners.findIndex((name) => name == action.name);
      return Object.assign({}, state, {
        party: Object.assign({}, state.party, {
          listeners: [...state.party.listeners.slice(0, indexOfListenerToRemove), ...state.party.listeners.slice(indexOfListenerToRemove + 1)],
        }),
      });
    }

    case LISTENER_JOINED: {
      return Object.assign({}, state, {
        party: Object.assign({}, state.party, {
          listeners: state.party.listeners != undefined ? [action.name, ...state.party.listeners] : [action.name],
        }),
      });
    }

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
      return Object.assign({}, state, {
        party: Object.assign({}, state.party, { nowPlaying: Object.assign({}, state.party.nowPlaying, action.track) }),
      });
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
      let indexOfSongToRemove = state.party.queue.findIndex((song) => song.id == action.song.id);
      return Object.assign({}, state, {
        party: Object.assign(
          {},
          state.party,
          { nowPlaying: Object.assign({}, { startPosition: action.position }, action.song) },
          {
            queue:
              indexOfSongToRemove == -1
                ? state.party.queue
                : [...state.party.queue.slice(0, indexOfSongToRemove), ...state.party.queue.slice(indexOfSongToRemove + 1)],
          },
          {
            history:
              state.party.nowPlaying && !state?.user?.details?.pausedMusic ? [...state.party.history, state.party.nowPlaying] : state.party.history,
          }
        ),
      });
    }

    case TOGGLE_PLAYBACK: {
      return Object.assign({}, state, {
        user: Object.assign({}, state.user, { details: Object.assign({}, state.user.details, { pausedMusic: !state.user.details.pausedMusic }) }),
      });
    }

    case UPDATE_USER_ACCESS_TOKEN: {
      return Object.assign({}, state, { user: Object.assign({}, state.user, { accessToken: action.accessToken }) });
    }

    case UPDATE_QUEUE: {
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
      return Object.assign({}, state, {
        user: Object.assign({}, state.user, { authentication: AUTHENTICATED }),
      });
    }

    case UPDATE_USER_DETAILS: {
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
        { party: action.isInParty ? Object.assign({}, { code: action.party.partyCode }, state.party) : state.party }
      );
    }

    case PARTY_JOINED: {
      return Object.assign(
        {},
        state,
        {
          user: Object.assign({}, state.user, { details: Object.assign({}, state.user.details, { isInParty: true }) }),
        },
        {
          party: Object.assign({}, { code: action.partyCode }, state.party, {
            host: action.host,
            listeners: state.party.listeners != undefined ? [...state.party.listeners, ...action.listeners] : action.listeners,
          }),
        }
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
export const isHost = (state) => state?.party?.host == state?.user?.details?.id && state?.party?.host != undefined;
export const getHost = (state) => state?.party?.host;
export const getListeners = (state) => state?.party?.listeners;
export const getCurrentSong = (state) => state?.party?.nowPlaying;
export const getParty = (state) => state.party;
export const getQueue = (state) => state?.party?.queue;
export const artistView = (state) => state?.views?.searchArtistId;
export const getTrackVotes = (state) => state?.party?.trackVotes;
export const getStartPosition = (state) => state?.party?.nowPlaying?.startPosition;
