import React from "react";
import styled from "styled-components";
import Header from "../shared/Header";

const $DiscoverFrame = styled.div``;

const DiscoverFrame = (props) => {
  return (
    <$DiscoverFrame className={props.className}>
      <Header>Discover</Header>
    </$DiscoverFrame>
  );
};

export default DiscoverFrame;
