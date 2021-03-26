import React from "react";
import styled from "styled-components";
import Button from "../shared/Button";
import { connect } from "react-redux";
import { createParty } from "../../api/party";
import { getRealtimeConnection } from "../../redux/reducers/reducers";

const $greyedOutBackground = styled.div`
  position: absolute;
  display: flex;
  align-items: center;
  justify-content: center;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: #000000;
  opacity: 0.7;
`;

const $popup = styled.div`
  background-color: #ffffff;
  border-radius: 10px;
  padding: 20px;
`;

const JoinOrCreateParty = (props) => {
  return (
    <$greyedOutBackground>
      <$popup>
        Join or Create Party
        <Button onClick={() => createParty(props.connection)(props.dispatch)}>Create Party</Button>
      </$popup>
    </$greyedOutBackground>
  );
};

const mapStateToProps = (state) => {
  return { connection: getRealtimeConnection(state).connection };
};

export default connect(mapStateToProps, null)(JoinOrCreateParty);
