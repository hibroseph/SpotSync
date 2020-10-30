import { ADD_CREATE_PLAYLIST_SONGS, TOGGLE_CREATE_PLAYLIST_SONG, LOG_IN, LOG_OUT } from "./ActionTypes";

export const addPlaylistSongs = (content) => ({
  type: ADD_CREATE_PLAYLIST_SONGS,
  payload: {
    content,
  },
});

export const togglePlaylistSong = (content) => ({
  type: TOGGLE_CREATE_PLAYLIST_SONG,
  payload: {
    content,
  },
});

export const logIn = (content) => ({
  type: LOG_IN,
  payload: {
    content,
  },
});

export const logOut = (content) => ({
  type: LOG_OUT,
  payload: {
    content,
  },
});
