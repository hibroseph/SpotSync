import styled from "styled-components";
import React from "react";

const $greyedOutBackground = styled.div`
  position: absolute;
  display: flex;
  align-items: center;
  justify-content: center;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.7);
  backdrop-filter: blur(10px);
  z-index: 1;
`;

const $popup = styled.div`
  background-color: #ffffff;
  border-radius: 10px;
  padding: 20px;
  max-width: 70%;
`;

export default ({ children }) => (
  <$greyedOutBackground>
    <$popup>{children}</$popup>
  </$greyedOutBackground>
);
