export const UPDATE_USER_DETAILS = "get_user_details";
export const UPDATE_USER_ACCESS_TOKEN = "update_user_access_token";
export const USERS_FAVORITE_SONGS = "users_favorite_songs";
export const FAVORITE_SONG = "favorite_song";

export const updateUserDetails = (isInParty, party, userDetails) => {
  return { type: UPDATE_USER_DETAILS, userDetails, isInParty, party };
};

export const setUserAccessToken = (accessToken) => {
  return {
    type: UPDATE_USER_ACCESS_TOKEN,
    accessToken,
  };
};
