import React from "react";
import styled from "styled-components";

const $ValidationFailure = styled.p`
  color: red;
  font-weight: strong;
`;
const ValidationFailure = (props) => {
  return <$ValidationFailure>{props.children}</$ValidationFailure>;
};

export default ValidationFailure;
