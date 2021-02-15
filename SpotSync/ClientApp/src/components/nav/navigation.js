import React from "react";
import { Button } from "../shared/button";
import styled from "styled-components";
import spotibroLogo from "../../../../wwwroot/assets/logo.svg";

const Nav = styled.nav`
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
  }
`;

const Navigation = (props) => {
  return (
    <Nav>
      <div className="left-nav-item">
        <img src={spotibroLogo}></img>
        <div style={{ "margin-left": "50px" }}>
          <Button title="Dashboard" link="/dashboard"></Button>
        </div>
      </div>
      <Button title="Login" link="/account/login"></Button>
    </Nav>
  );
};

export { Navigation };
