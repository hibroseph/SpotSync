import React, { useEffect, useState } from "react";
import styled from "styled-components";
import Tabs from "./Tabs";
import Queue from "./Queue/Queue";
import History from "./History/History";
import { getUserLikesDislikes } from "../../api/party";
import { connect } from "react-redux";
import { getPartyCode, getSongFeelings, isHost } from "../../redux/reducers/reducers";
import { setSongFeelings } from "../../redux/actions/party";
import Listeners from "./Listeners";
import $ScrollContainer from "../shared/ScrollContainer";
import OutlinedContainer from "../shared/OutlinedContainer";

const $Sidebar = styled(OutlinedContainer)`
  margin: 0 5px 5px 0;
  display: flex;
  flex-direction: column;
  overflow-y: auto;
`;

const $SidebarContent = styled.div`
  padding: 5px;
  display: flex;
  flex-direction: column;
  align-items: center;
  overflow-y: auto;

  &::-webkit-scrollbar {
    width: 10px;
  }

  &::-webkit-scrollbar-thumb {
    background-color: #e1e1e1;
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

const Sidebar = ({ partyCode, className, songFeelings = {}, dispatch, isHost }) => {
  const [currentTabView, setTabView] = useState("Queue");

  useEffect(() => {
    if (partyCode != undefined) {
      getUserLikesDislikes(partyCode).then((res) => {
        dispatch(setSongFeelings(ConvertUserLikesDislikesFromServerToClient(res)));
      });
    }
  }, [partyCode]);

  const GetSideBarContent = () => {
    switch (currentTabView) {
      case "Queue":
        return <Queue songFeelings={songFeelings} isHost={isHost}></Queue>;
      case "History":
        return <History></History>;
      case "Listeners":
        return <Listeners />;
    }
  };

  return (
    <$Sidebar className={className}>
      <Tabs tabs={tabs} selected={currentTabView} changeSelectedTab={setTabView}></Tabs>
      <$ScrollContainer>
        <$SidebarContent>{GetSideBarContent()}</$SidebarContent>
      </$ScrollContainer>
    </$Sidebar>
  );
};

const mapStateToProps = (state) => {
  return {
    partyCode: getPartyCode(state),
    songFeelings: getSongFeelings(state),
    isHost: isHost(state),
  };
};

export default connect(mapStateToProps, null)(Sidebar);
