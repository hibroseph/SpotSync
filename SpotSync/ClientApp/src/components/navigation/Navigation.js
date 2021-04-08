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

const $ButtonGroup = styled.div`
  display: flex;
  width: 275px;
  justify-content: space-between;
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

const Navigation = ({ partyCode, user, dispatch }) => {
  return (
    <$Navigation>
      <div className="left-nav-item">
        <img className="logo" src={spotibroLogo}></img>
        <div className="button-spacing">{/*<LinkButton title="Dashboard" link="/dashboard"></LinkButton>*/}</div>
      </div>
      <Subtitle>{partyCode}</Subtitle>
      <$ButtonGroup>
        {user?.details?.isInParty && (
          <React.Fragment>
            <Button selected onClick={() => leaveParty(partyCode)(dispatch)}>
              Leave Party
            </Button>
          </React.Fragment>
        )}
        {user.authentication == AUTHENTICATED && <LinkButton title="Logout" link="/account/logout"></LinkButton>}
        {user.authentication == UNAUTHENTICATED && <LinkButton title="Login" link="/account/login"></LinkButton>}
      </$ButtonGroup>
    </$Navigation>
  );
};

const mapStateToProps = (state) => {
  return { user: getUser(state), partyCode: getPartyCode(state) };
};

export default connect(mapStateToProps, null)(Navigation);
