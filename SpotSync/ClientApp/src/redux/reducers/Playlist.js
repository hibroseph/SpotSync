import { ADD_CREATE_PLAYLIST_SONGS, TOGGLE_CREATE_PLAYLIST_SONG } from "../ActionTypes";

const initialState = {
  createPlaylistSongs: [],
};

export default (state = initialState, action) => {
  switch (action.type) {
    case ADD_CREATE_PLAYLIST_SONGS: {
      return { ...state, createPlaylistSongs: [...action.payload] };
    }
    case TOGGLE_CREATE_PLAYLIST_SONG: {
      const newState = {
        ...state,
        createPlaylistSongs: state.createPlaylistSongs.map((song) => {
          if (song.trackUri === action.payload.trackUri) {
            return action.payload;
          } else {
            return song;
          }
        }),
      };
      return newState;
    }
    default:
      return state;
  }
};
