import React, { useEffect, useState } from "react";
import styled from "styled-components";
import Tabs from "./Tabs";
import Queue from "./Queue/Queue";

const $Sidebar = styled.div``;

const $SidebarContent = styled.div`
  background-color: #e5e5e5;
  border-radius: 10px;
  padding: 5px;
  display: flex;
  flex-direction: column;
  align-items: center;
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
    console.log("updating tab with " + tab);
    console.log(tab);
    setTabView(tab);
  };

  const GetSideBarContent = () => {
    console.log("getting sidebar content");
    switch (currentTabView) {
      case "Queue":
        return <Queue></Queue>;
      case "History":
        return <p>Not Implemented</p>;
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
