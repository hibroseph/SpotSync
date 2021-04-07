import React from "react";
import styled from "styled-components";

const $Input = styled.input`
  padding: 12px 20px;
  border-radius: 10px;
  box-sizing: border-box;
  border: 3px solid #fafafa;
`;

const Input = (props) => {
  return <$Input {...props}>{props.children}</$Input>;
};

export default Input;
