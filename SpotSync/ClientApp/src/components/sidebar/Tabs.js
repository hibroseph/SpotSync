import React from "react";
import styled from "styled-components";
import TabButton from "../shared/TabButton";

const $Tabs = styled.div`
  display: flex;
  justify-content: space-around;
  padding: 10px;
`;

const Tabs = ({ tabs, selected, changeSelectedTab }) => {
  return (
    <$Tabs>
      {tabs.map((tab) => {
        return (
          <TabButton selected={tab.title == selected} key={tab.title} onClick={() => changeSelectedTab(tab.title)}>
            {tab.title}
          </TabButton>
        );
      })}
    </$Tabs>
  );
};

export default Tabs;
