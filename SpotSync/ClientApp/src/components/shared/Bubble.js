import React from "react";
import styled from "styled-components";

const $bubble = styled.div`
  background-color: ${(props) => props.color};
  margin: 5px;
  font-size: 12px;
  padding: 2px 5px;
  border-radius: 10px;
`;
export default ({ color, children }) => {
  return <$bubble color={color}>{children}</$bubble>;
};
