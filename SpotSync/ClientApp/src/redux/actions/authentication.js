export const CHECKING_AUTHENTICATION = "CHECKING_AUTHENTICATION";
export const IS_AUTHENTICATED = "IS_AUTHENTICATED";
export const IS_NOT_AUTHENTICATED = "IS_NOT_AUTHENTICATED";

export const checkingAuthentication = () => {
  return {
    type: CHECKING_AUTHENTICATION,
  };
};

export const isAuthenticated = (userName) => {
  return {
    type: IS_AUTHENTICATED,
    userName,
  };
};

export const isNotAuthenticated = () => {
  return {
    type: IS_NOT_AUTHENTICATED,
  };
};
