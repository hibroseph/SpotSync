import React from "react";
import styled from "styled-components";
import Button from "../shared/Button";

const $Tabs = styled.div`
  display: flex;
  justify-content: space-around;
  padding: 10px;
`;

const Tabs = (props) => {
  return (
    <$Tabs>
      {props.tabs.map((tab) => {
        return (
          <Button selected={tab.title == props.selected} key={tab.title} onClick={() => props.onClick(tab.title)}>
            {tab.title}
          </Button>
        );
      })}
    </$Tabs>
  );
};

export default Tabs;
