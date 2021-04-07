import React, { useState } from "react";
import styled from "styled-components";
import Button from "../shared/Button";
import Input from "../shared/Input";
import ValidationFailure from "../shared/ValidationFailure";
import { connect } from "react-redux";
import { createParty, joinParty } from "../../api/party";
import { getRealtimeConnection } from "../../redux/reducers/reducers";
import Loader from "../shared/Loader";

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

const $flexContainer = styled.div`
  display: flex;
  width: 100%;
  padding: 20px;
  box-sizing: border-box;
  justify-content: center;
`;

const $marginedButton = styled(Button)`
  margin: 10px;
`;

const $verticalBar = styled.div`
  border-left: 2px solid #e2e2e2;
`;

const $styledInput = styled(Input)`
  margin-left: 10px;
`;

const JoinParty = (partyCode, setLoading, connection) => {
  setLoading(true);
  return joinParty(partyCode, connection);
};

const JoinOrCreateParty = ({ connection, dispatch }) => {
  const [isLoading, setLoading] = useState(false);
  const [partyCode, setPartyCode] = useState(null);
  const [failureMessage, setFailureMessage] = useState(null);

  return (
    <$greyedOutBackground>
      <$popup>
        Join or Create Party
        <$flexContainer>
          {isLoading && <Loader isLoading={isLoading}></Loader>}
          {!isLoading && (
            <React.Fragment>
              <$marginedButton onClick={() => createParty(connection)(dispatch)}>Create Party</$marginedButton>
              <$verticalBar />
              <div>
                <$styledInput onInput={(event) => setPartyCode(event.target.value)} placeholder="Party Code"></$styledInput>
                <$marginedButton
                  onClick={() => {
                    JoinParty(partyCode, setLoading, connection).then((result) => {
                      setLoading(false);

                      if (!result.succeeded) {
                        setFailureMessage(result.message);
                      }
                    });
                  }}
                >
                  Join
                </$marginedButton>
              </div>
            </React.Fragment>
          )}
        </$flexContainer>
        <$flexContainer>{failureMessage && <ValidationFailure>{failureMessage}</ValidationFailure>}</$flexContainer>
      </$popup>
    </$greyedOutBackground>
  );
};

const mapStateToProps = (state) => {
  return { connection: getRealtimeConnection(state).connection };
};

export default connect(mapStateToProps, null)(JoinOrCreateParty);
