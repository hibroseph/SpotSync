export const createParty = () => {
  return (dispatch) => {
    fetch("/party/startParty", {
      method: "POST",
    })
      .then((res) => res.json())
      .then((json) => {
        console.log(json);
        dispatch(partyJoined(json.isInParty, json.party, json.userDetails));
      });
  };
};
