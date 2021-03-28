import React from "react";
import styled from "styled-components";

const $Input = styled.input`
  border: none;
  background-color: #e2e2e2;
  padding: 10px;
  border-radius: 5px;
`;

const Input = (props) => {
  return <$Input {...props}>{props.children}</$Input>;
};

export default Input;
