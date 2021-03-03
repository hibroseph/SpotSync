import React, { useEffect, useState } from "react";
import Navigation from "./components/navigation/Navigation";
import MainContent from "./components/main/MainContent";
import connectToPartyHub, { setupPartyHub } from "./signalR/setupPartyHub";
import { checkIfAuthenticated } from "./api/authentication";
import { fetchUserDetails } from "./api/user";
import { connect } from "react-redux";
import { getUser } from "./redux/reducers/reducers";
import { JoinOrCreateParty } from "./components/main/JoinOrCreateParty";

function App(props) {
  console.log("The App is starting up");
  console.log(props);

  useEffect(() => {
    console.log("verify user is authenticated");
    checkIfAuthenticated()(props.dispatch);
    fetchUserDetails()(props.dispatch);
    console.log("Setting up party hub");
    setupPartyHub();
    connectToPartyHub();
  }, []);

  useEffect(() => {
    console.log("user details has changed");
    console.log(props);
  }, props?.userDetails?.isInParty);

  return (
    <React.Fragment>
      {!props?.userDetails?.isInParty && <JoinOrCreateParty></JoinOrCreateParty>}
      <Navigation />
      <MainContent></MainContent>
    </React.Fragment>
  );
}

const mapStateToProps = (state) => {
  return { userDetails: getUser(state)?.details };
};

export default connect(mapStateToProps, null)(App);
