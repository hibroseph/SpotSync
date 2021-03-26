import React from "react";
import styled from "styled-components";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlayCircle, faStepForward, faPauseCircle } from "@fortawesome/free-solid-svg-icons";
import { connect } from "react-redux";
import { getUser, getPartyCode, getRealtimeConnection, getCurrentSong } from "../../../redux/reducers/reducers";
import { togglePlaybackState } from "../../../api/party";
import { skipSong } from "../../../api/partyHub";

const $NowPlaying = styled.div`
  box-sizing: border-box;
  width: 100%;
  bottom: 0px;
  left: 0;
  padding: 10px;
  background-color: #e5e5e5;
  display: flex;
  justify-content: space-around;

  flex: 0 1 30px;
`;

const $SongManagement = styled.div`
  display: flex;
  align-items: center;
`;

const $PlayFontAwesomeIcon = styled(FontAwesomeIcon)`
  font-size: 30px;

  &:hover {
    color: grey;
  }
`;

const $SkipFontAwesomeIcon = styled(FontAwesomeIcon)`
  font-size: 20px;

  &:hover {
    color: grey;
  }
`;

const $NowPlayingSong = styled.div`
  display: flex;

  .song-information {
    display: flex;
    flex-direction: column;
    justify-content: center;
    padding: 10px;
  }
  p {
    margin: 0px;
    font-size: 10px;
  }

  img {
    width: 50px;
  }
`;

const NowPlaying = ({ user, partyCode, dispatch, connection, currentSong }) => {
  return (
    <$NowPlaying>
      <$NowPlayingSong>
        <img src={currentSong?.albumImageUrl} />
        <div class="song-information">
          <p>{currentSong?.name}</p>
          <p>{currentSong?.artist}</p>
        </div>
      </$NowPlayingSong>
      <$SongManagement>
        {user?.details?.pausedMusic ? (
          <$PlayFontAwesomeIcon icon={faPlayCircle} onClick={() => togglePlaybackState(partyCode, dispatch)} />
        ) : (
          <$PlayFontAwesomeIcon icon={faPauseCircle} onClick={() => togglePlaybackState(partyCode, dispatch)} />
        )}
        <$SkipFontAwesomeIcon icon={faStepForward} onClick={() => skipSong(partyCode, connection)}></$SkipFontAwesomeIcon>
      </$SongManagement>
      <div></div>
    </$NowPlaying>
  );
};

const mapStateToProps = (state) => {
  return {
    user: getUser(state),
    partyCode: getPartyCode(state),
    connection: getRealtimeConnection(state).connection,
    currentSong: getCurrentSong(state),
  };
};

export default connect(mapStateToProps, null)(NowPlaying);
