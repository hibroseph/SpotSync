import React from "react";
import styled from "styled-components";
import Button from "../shared/Button";

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

export const JoinOrCreateParty = (props) => {
  return (
    <$greyedOutBackground>
      <$popup>
        Join or Create Party
        <Button>Create Party</Button>
      </$popup>
    </$greyedOutBackground>
  );
};
