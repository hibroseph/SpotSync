import React from "react";
import Button from "./Button";
import styled from "styled-components";

const $TabButton = styled(Button)`
  background-color: ${(props) => (props.selected ? "#FBD277" : "white")};
  color: ${(props) => (props.selected ? "white" : "#AAAAAA")};
  border-radius: 100px;

  &:hover {
    background-color: #fbd277;
    filter: opacity(50%);
    color: white;
  }
`;

const TabButton = ({ onClick, selected, children }) => {
  return (
    <$TabButton onClick={onClick} selected={selected}>
      {children}
    </$TabButton>
  );
};

export default TabButton;
