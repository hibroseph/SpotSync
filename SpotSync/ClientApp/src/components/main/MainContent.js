import React from "react";
import styled from "styled-components";
import DiscoverFrame from "../discover/DiscoverFrame";
import Sidebar from "../sidebar/Sidebar";

const $MainContent = styled.div`
  display: flex;
  width: 100%;
  flex-direction: row;
  flex: 1 1 auto;
  overflow-y: auto;
  padding: 10px 20px;
  box-sizing: border-box;
`;

const $SidebarWithWidth = styled(Sidebar)`
  && {
    width: 25%;
  }
`;

const MainContent = ({ contributions, showContributionsPopup, setContributions }) => {
  return (
    <$MainContent>
      <DiscoverFrame showContributionsPopup={showContributionsPopup} contributions={contributions} setContributions={setContributions} />
      <$SidebarWithWidth />
    </$MainContent>
  );
};

export default MainContent;
