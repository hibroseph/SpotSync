import React from "react";
import styled from "styled-components";

export default styled.div`
  overflow-y: auto;
  &::-webkit-scrollbar {
    width: 10px;
  }

  &::-webkit-scrollbar-thumb {
    background-color: #e1e1e1;
    border-radius: 10px;
  }
`;
