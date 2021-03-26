import React, { useEffect, useState } from "react";
import styled from "styled-components";
import Tabs from "./Tabs";
import Queue from "./Queue/Queue";
import History from "./History/History";

const $Sidebar = styled.div`
  margin: 0 5px 5px 0;
  display: flex;
  flex-direction: column;
  overflow-y: auto;
`;

const $SidebarContent = styled.div`
  background-color: #e5e5e5;
  border-radius: 10px;
  padding: 5px;
  display: flex;
  flex-direction: column;
  align-items: center;
  overflow-y: auto;

  &::-webkit-scrollbar {
    background-color: #e5e5e5;
    width: 10px;
  }

  &::-webkit-scrollbar-thumb {
    background-color: grey;
    border-radius: 10px;
  }
`;

const tabs = [
  {
    title: "Queue",
  },
  {
    title: "History",
  },
  {
    title: "Listeners",
  },
];

const Sidebar = (props) => {
  const [currentTabView, setTabView] = useState("Queue");

  const changeTabView = (tab) => {
    setTabView(tab);
  };

  const GetSideBarContent = () => {
    switch (currentTabView) {
      case "Queue":
        return <Queue></Queue>;
      case "History":
        return <History></History>;
      case "Listeners":
        return <p>Not Implemented</p>;
    }
  };

  return (
    <$Sidebar className={props.className}>
      <Tabs tabs={tabs} selected={currentTabView} onClick={(tab) => changeTabView(tab)}></Tabs>
      <$SidebarContent>{GetSideBarContent()}</$SidebarContent>
    </$Sidebar>
  );
};

export default Sidebar;
