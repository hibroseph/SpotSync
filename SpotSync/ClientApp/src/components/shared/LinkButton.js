import React from "react";
import styled from "styled-components";

const $LinkButton = styled.a`
  border-radius: 10px;
  background-color: #e5e5e5;
  border: none;
  font-weight: bold;
  font-size: 18px;
  padding: 10px;
  color: black;
  text-decoration: none; /* no underline */

  &:hover {
    background-color: #e0e0e0;
  }
`;

const LinkButton = (props) => {
  return <$LinkButton href={props.link}>{props.title}</$LinkButton>;
};

export default LinkButton;
