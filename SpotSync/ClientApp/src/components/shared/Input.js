import React from "react";
import styled from "styled-components";

const $Input = styled.input`
  padding: 12px 20px;
  border-radius: 25px;
  box-sizing: border-box;
  border: 3px solid #e1e1e1;
  height: 50px;
  font-size: 19px;
`;

const Input = (props) => {
  return <$Input {...props}>{props.children}</$Input>;
};

export default Input;
