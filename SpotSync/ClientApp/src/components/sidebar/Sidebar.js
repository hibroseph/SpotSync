import React, { useEffect, useState } from "react";
import styled from "styled-components";
import Tabs from "./Tabs";
import Queue from "./Queue/Queue";
import History from "./History/History";
import { getUserLikesDislikes } from "../../api/party";
import { connect } from "react-redux";
import { getPartyCode } from "../../redux/reducers/reducers";

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

  console.log("AFTER CONVERSION");
  console.log(songFeelings);
  return songFeelings;
};

const Sidebar = ({ partyCode, className }) => {
  const [currentTabView, setTabView] = useState("Queue");
  const [songFeelings, setSongFeelings] = useState({});

  useEffect(() => {
    console.log("song feelings");
    console.log(songFeelings);
  }, [songFeelings]);

  useEffect(() => {
    if (partyCode != undefined) {
      console.log("getting users lieks");
      getUserLikesDislikes(partyCode).then((res) => {
        console.log("got the users likes ");
        console.log(res);
        setSongFeelings(ConvertUserLikesDislikesFromServerToClient(res));
      });
    }
  }, [partyCode]);

  const changeTabView = (tab) => {
    setTabView(tab);
  };

  const GetSideBarContent = () => {
    switch (currentTabView) {
      case "Queue":
        return <Queue songFeelings={songFeelings} setSongFeelings={setSongFeelings}></Queue>;
      case "History":
        return <History songFeelings={songFeelings} setSongFeelings={setSongFeelings}></History>;
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
  };
};
export default connect(mapStateToProps, null)(Sidebar);
