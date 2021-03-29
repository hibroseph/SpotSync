import React from "react";
import styled from "styled-components";

const $centered = styled.div`
  display: flex;
  width: 100%;
  justify-content: center;
  padding: 10px;
`;

export default (props) => {
  return <$centered>{props.children}</$centered>;
};
