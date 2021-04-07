import React from "react";
import LinkButton from "../shared/LinkButton";
import styled from "styled-components";
import spotibroLogo from "../../../../wwwroot/assets/logo.svg";
import { connect } from "react-redux";
import { getUser, getPartyCode } from "../../redux/reducers/reducers";
import { AUTHENTICATED, UNAUTHENTICATED } from "../../states/authentication";
import { leaveParty } from "../../api/party";
import Subtitle from "../shared/Subtitle";

import Button from "../shared/Button";

const LeavePartyButton = styled(Button)`
  background-color: #f2b727;
  color: white;
  margin: 0px 10px;

  &:hover {
    background-color: #e2b727;
  }
`;

const $Navigation = styled.nav`
  padding: 10px 30px;
  box-sizing: border-box;
  width: 100%;
  display: flex;
  justify-content: space-between;
  align-items: center;

  flex: 0 1 auto;

  .left-nav-item {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .logo {
    height: 40px;
    margin-right: 25px;
  }

  .button-spacing > a {
    margin: 0px 5px;
  }
`;

const Navigation = (props) => {
  return (
    <$Navigation>
      <div className="left-nav-item">
        <img className="logo" src={spotibroLogo}></img>
        <div className="button-spacing">{/*<LinkButton title="Dashboard" link="/dashboard"></LinkButton>*/}</div>
      </div>
      <Subtitle>{props?.partyCode}</Subtitle>
      <div>
        {props?.user?.details?.isInParty && (
          <React.Fragment>
            <LeavePartyButton selected onClick={() => leaveParty(props.partyCode)(props.dispatch)}>
              Leave Party
            </LeavePartyButton>
          </React.Fragment>
        )}
        {props.user.authentication == AUTHENTICATED && <LinkButton title="Logout" link="/account/logout"></LinkButton>}
        {props.user.authentication == UNAUTHENTICATED && <LinkButton title="Login" link="/account/login"></LinkButton>}
      </div>
    </$Navigation>
  );
};

const mapStateToProps = (state) => {
  return { user: getUser(state), partyCode: getPartyCode(state) };
};

export default connect(mapStateToProps, null)(Navigation);
