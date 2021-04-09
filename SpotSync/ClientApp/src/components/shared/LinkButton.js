import React from "react";
import styled from "styled-components";

const $LinkButton = styled.a`
  border-radius: 10px;
  background-color: #7d8aff;
  border: none;
  font-size: 18px;
  padding: 10px 25px;
  font-weight: bold;
  color: white;
  text-decoration: none;

  &:hover {
    filter: opacity(50%);
  }
`;

const LinkButton = (props) => {
  return <$LinkButton href={props.link}>{props.title}</$LinkButton>;
};

export default $LinkButton;
