/** @jsx jsx */
import { jsx } from "theme-ui";
import Nav from "./components/Nav";
import AvailableParties from "./components/AvailableParties";
import JoinParty from "./components/JoinParty";
import CreateParty from "./components/CreateParty";
import { useDispatch } from "react-redux";
import { useEffect } from "react";
import { IsUserAuthenticated } from "./redux/Thunks";
import { BrowserRouter as Router, Switch, Route, BrowserRouter } from "react-router-dom";
import NowPlaying from "./components/NowPlaying";
import Members from "./components/Members";

function App(props) {
  const dispatch = useDispatch();
  useEffect(() => {
    dispatch(IsUserAuthenticated());
  }, []);

  return (
    <BrowserRouter>
      <Route exact path="/">
        <Nav></Nav>
        <p>Sign In</p>
      </Route>
      <Route exact path="/dashboard">
        <Nav></Nav>
        <div {...props}>
          <AvailableParties />
          <div
            sx={{
              display: "flex",
              flexDirection: ["column", "row"],
              width: ["99%", "80%", "70%", "60%"],
            }}
          >
            <JoinParty></JoinParty>
            <CreateParty></CreateParty>
          </div>
        </div>
      </Route>
      <Route exact path="/currentparty">
        <Nav></Nav>
        <div {...props}>
          <div
            sx={{
              display: "flex",
              flexDirection: ["column", "row"],
              width: ["99%", "80%", "70%", "60%"],
            }}
          >
            <NowPlaying></NowPlaying>
            <Members></Members>
          </div>
        </div>
      </Route>
    </BrowserRouter>
  );
}

export default App;
