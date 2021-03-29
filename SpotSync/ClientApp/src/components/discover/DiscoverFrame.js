import React, { useState, useEffect } from "react";
import styled from "styled-components";
import Header from "../shared/Header";
import Search from "../discover/Search/Search";
const $DiscoverFrame = styled.div``;

const DiscoverFrame = (props) => {
  return (
    <$DiscoverFrame className={props.className}>
      <Header>Discover</Header>
      <Search></Search>
    </$DiscoverFrame>
  );
};

export default DiscoverFrame;
