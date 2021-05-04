import React from "react";
import styled from "styled-components";

const $contribution = styled.div`
  background-color: ${(props) => {
    switch (props.type) {
      case 0: // track
        return "#42B0FF;";
      case 1: // artist
        return "#5ABE6A;";
      case 2: // album
        return "#FFAC5F;";
      case 3: // playlist
        return "#D55FFF;";
      default:
        // grey
        return "#E5E5E5;";
    }
  }}
    &:hover {
        transform: scale(1.1);
    }

    color: ${(props) => {
      switch (props.type) {
        case 0: // track
        case 1: // artist
        case 2: // album
        case 3: // playlist
          return "#FFFFFF;";
        default:
          // grey
          return "#000000;";
      }
    }}

    transition: all .2s ease-in-out;
    
    border-radius: 50px;
    padding: 5px 10px 5px 10px;
    margin: 3px 9px;
`;

export default ({ name, id, type, actOnContribution, children }) => {
  return (
    <$contribution type={type} onClick={() => actOnContribution({ name, id, type })}>
      {name}
      {children}
    </$contribution>
  );
};
