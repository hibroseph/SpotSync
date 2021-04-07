import React from "react";
import styled from "styled-components";
import DiscoverFrame from "../discover/DiscoverFrame";
import Sidebar from "../sidebar/Sidebar";
import { ToastContainer, toast } from "react-toastify";

const $MainContent = styled.div`
  display: flex;
  width: 100%;
  flex-direction: row;
  flex: 1 1 auto;
  overflow-y: auto;
  padding: 10px 20px;
  box-sizing: border-box;
`;

const $WideDiscoverFrame = styled(DiscoverFrame)`
  && {
    padding: 0 10px;
    width: 70%;
    padding: 0 30px 20px 10px;
  }
`;

const $SidebarWithWidth = styled(Sidebar)`
  && {
    width: 30%;
  }
`;

const MainContent = (props) => {
  return (
    <$MainContent>
      <$WideDiscoverFrame></$WideDiscoverFrame>
      <$SidebarWithWidth></$SidebarWithWidth>
    </$MainContent>
  );
};

export default MainContent;
