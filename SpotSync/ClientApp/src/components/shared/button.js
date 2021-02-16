import React from "react";
import styled from "styled-components";

const $Button = styled.button`
  border-radius: 10px;
  background-color: ${(props) => props.selected && "#e5e5e5"};
  border: none;
  font-weight: bold;
  padding: 10px;

  &:hover {
    background-color: #e0e0e0;
  }
`;

const Button = (props) => {
  return (
    <$Button selected={props.selected} className={props.className} onClick={props.onClick}>
      {props.children}
    </$Button>
  );
};

export default Button;
