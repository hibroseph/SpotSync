export const UPDATE_USER_DETAILS = "get_user_details";

export const updateUserDetails = (isInParty, party, userDetails) => {
  return { type: UPDATE_USER_DETAILS, userDetails, isInParty, party };
};
