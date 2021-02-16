import React, { useEffect, useState } from "react";
import styled from "styled-components";
import Tabs from "./Tabs";

const $Sidebar = styled.div``;

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

  return (
    <$Sidebar className={props.className}>
      <Tabs tabs={tabs} selected={currentTabView} onClick={(tab) => changeTabView(tab)}></Tabs>
    </$Sidebar>
  );
};

export default Sidebar;
