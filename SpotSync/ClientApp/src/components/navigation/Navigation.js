import React from "react";
import LinkButton from "../shared/LinkButton";
import styled from "styled-components";
import spotibroLogo from "../../../../wwwroot/assets/logo.svg";
import { connect } from "react-redux";
import { getUser } from "../../redux/reducers/reducers";
import { AUTHENTICATED, UNAUTHENTICATED } from "../../states/authentication";
const $Navigation = styled.nav`
  padding: 10px 30px;
  box-sizing: border-box;
  width: 100%;
  height: 50px;
  display: flex;
  justify-content: space-between;
  align-items: center;

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
  console.log("props in navigation");
  console.log(props);
  return (
    <$Navigation>
      <div className="left-nav-item">
        <img className="logo" src={spotibroLogo}></img>
        <div className="button-spacing">
          <LinkButton title="Dashboard" link="/dashboard"></LinkButton>
        </div>
      </div>
      {props.user.authentication == AUTHENTICATED && <LinkButton title="Logout" link="/account/logout"></LinkButton>}
      {props.user.authentication == UNAUTHENTICATED && <LinkButton title="Login" link="/account/login"></LinkButton>}
    </$Navigation>
  );
};

const mapStateToProps = (state) => {
  return { user: getUser(state) };
};

export default connect(mapStateToProps, null)(Navigation);
