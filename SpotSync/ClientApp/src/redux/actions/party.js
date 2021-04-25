export const PARTY_JOINED = "party_joined";
export const LEFT_PARTY = "left_party";
export const UPDATE_QUEUE = "update_queue";
export const UPDATE_HISTORY = "update_history";
export const TOGGLE_PLAYBACK = "toggle_playback";
export const UPDATE_SONG = "update_song";
export const UPDATE_CURRENT_SONG = "update_current_song";
export const USER_LIKES_SONG = "user_likes_song";
export const USER_DISLIKES_SONG = "user_dislikes_song";
export const SET_SONG_FEELINGS = "set_song_feelings";
export const LISTENER_JOINED = "listener_joined";
export const LISTENER_LEFT = "listener_left";
export const UPDATE_TRACK_VOTES = "update_track_votes";
export const UPDATE_POSITION_IN_SONG = "update_position_in_song";

export const updatePositionInSong = (position) => {
  return {
    type: UPDATE_POSITION_IN_SONG,
    position,
  };
};

export const listenerLeft = (name) => {
  return {
    type: LISTENER_LEFT,
    name,
  };
};

export const listenerJoined = (name) => {
  return {
    type: LISTENER_JOINED,
    name,
  };
};

export const setSongFeelings = (songFeelings) => {
  return {
    type: SET_SONG_FEELINGS,
    songFeelings,
  };
};

export const partyLeft = () => {
  return {
    type: LEFT_PARTY,
  };
};

export const userLikesSong = (trackUri) => {
  return {
    type: USER_LIKES_SONG,
    trackUri,
  };
};

export const userDislikesSong = (trackUri) => {
  return {
    type: USER_DISLIKES_SONG,
    trackUri,
  };
};

export const updateCurrentSong = (track) => {
  return {
    type: UPDATE_CURRENT_SONG,
    track,
  };
};

export const partyJoined = (partyCode, listeners, host) => {
  return {
    type: PARTY_JOINED,
    partyCode,
    listeners,
    host,
  };
};

export const updateQueue = (queue) => {
  return {
    type: UPDATE_QUEUE,
    queue,
  };
};

export const updateHistory = (history) => {
  return {
    type: UPDATE_HISTORY,
    history,
  };
};

export const togglePlayback = () => {
  return {
    type: TOGGLE_PLAYBACK,
  };
};

export const updateSong = (song, position) => {
  return {
    type: UPDATE_SONG,
    position,
    song,
  };
};

export const updateTrackVotes = (trackVotes) => {
  return {
    type: UPDATE_TRACK_VOTES,
    trackVotes,
  };
};
