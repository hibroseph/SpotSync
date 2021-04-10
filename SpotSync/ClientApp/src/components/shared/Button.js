import React from "react";
import styled from "styled-components";

const $Button = styled.button`
  border-radius: 10px;
  background-color: #7d8aff;
  border: none;
  font-size: 18px;
  padding: 10px 25px;
  font-weight: bold;
  color: white;

  &:hover {
    filter: opacity(50%);
  }
`;

export default $Button;
