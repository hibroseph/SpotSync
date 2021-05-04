import React, { useEffect, useState } from "react";
import styled from "styled-components";
import Button from "../shared/Button";
import Input from "../shared/Input";
import ValidationFailure from "../shared/ValidationFailure";
import { connect } from "react-redux";
import { createParty, joinParty } from "../../api/party";
import { getRealtimeConnection } from "../../redux/reducers/reducers";
import Loader from "../shared/Loader";
import Popup from "../shared/Popup";
import Title from "../shared/Title";

const $flexContainer = styled.div`
  display: flex;
  width: 100%;
  padding: 20px;
  box-sizing: border-box;
  justify-content: center;
  align-items: center;
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

const JoinOrCreateParty = ({ connection, showContributionsPopup }) => {
  const [isLoading, setLoading] = useState(true);
  const [partyCode, setPartyCode] = useState(null);
  const [failureMessage, setFailureMessage] = useState(null);

  useEffect(() => {
    if (connection != undefined) {
      setLoading(false);
    }
  }, [connection]);
  return (
    <Popup>
      <Title>Join or Create Party</Title>
      <$flexContainer>
        {isLoading && <Loader isLoading={isLoading}></Loader>}
        {!isLoading && (
          <React.Fragment>
            <$marginedButton
              onClick={() =>
                createParty(connection).then(() => {
                  showContributionsPopup(true);
                })
              }
            >
              Create Party
            </$marginedButton>
            <$verticalBar />
            <div>
              <$styledInput onInput={(event) => setPartyCode(event.target.value)} placeholder="Party Code"></$styledInput>
              <$marginedButton
                onClick={() => {
                  JoinParty(partyCode, setLoading, connection).then((result) => {
                    setLoading(false);
                    showContributionsPopup(true);
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
    </Popup>
  );
};

const mapStateToProps = (state) => {
  return { connection: getRealtimeConnection(state).connection };
};

export default connect(mapStateToProps, null)(JoinOrCreateParty);
