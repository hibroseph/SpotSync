import React, { useEffect, useState } from "react";
import styled from "styled-components";
import Tabs from "./Tabs";
import Queue from "./Queue/Queue";
import History from "./History/History";
import { getUserLikesDislikes } from "../../api/party";
import { connect } from "react-redux";
import { getPartyCode, getSongFeelings } from "../../redux/reducers/reducers";
import { setSongFeelings } from "../../redux/actions/party";

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

const ConvertUserLikesDislikesFromServerToClient = (res) => {
  let songFeelings = {};

  res.dislikedSongs.map((dislikedTrackUri) => (songFeelings = Object.assign({}, songFeelings, { [dislikedTrackUri]: 0 })));
  res.likedSongs.map((likedTrackUri) => (songFeelings = Object.assign({}, songFeelings, { [likedTrackUri]: 1 })));
  return songFeelings;
};

const Sidebar = ({ partyCode, className, songFeelings, dispatch }) => {
  const [currentTabView, setTabView] = useState("Queue");

  useEffect(() => {
    if (partyCode != undefined) {
      getUserLikesDislikes(partyCode).then((res) => {
        dispatch(setSongFeelings(ConvertUserLikesDislikesFromServerToClient(res)));
      });
    }
  }, [partyCode]);

  const changeTabView = (tab) => {
    setTabView(tab);
  };

  const GetSideBarContent = () => {
    switch (currentTabView) {
      case "Queue":
        return <Queue songFeelings={songFeelings}></Queue>;
      case "History":
        return <History songFeelings={songFeelings}></History>;
      case "Listeners":
        return <p>Not Implemented</p>;
    }
  };

  return (
    <$Sidebar className={className}>
      <Tabs tabs={tabs} selected={currentTabView} onClick={(tab) => changeTabView(tab)}></Tabs>
      <$SidebarContent>{GetSideBarContent()}</$SidebarContent>
    </$Sidebar>
  );
};

const mapStateToProps = (state) => {
  return {
    partyCode: getPartyCode(state),
    songFeelings: getSongFeelings(state),
  };
};
export default connect(mapStateToProps, null)(Sidebar);
