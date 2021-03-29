import React from "react";
import styled from "styled-components";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlayCircle, faStepForward, faPauseCircle } from "@fortawesome/free-solid-svg-icons";
import { connect } from "react-redux";
import { getUser, getPartyCode, getRealtimeConnection, getCurrentSong, getSongFeelings } from "../../../redux/reducers/reducers";
import { togglePlaybackState } from "../../../api/party";
import { skipSong } from "../../../api/partyHub";
import { userLikesSong, userDislikesSong } from "../../../api/partyHub";
import ThumbsUp from "../../shared/ThumbsUp";
import ThumbsDown from "../../shared/ThumbsDown";
import Loader from "../../shared/Loader";

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
  flex: 1;
  align-items: center;
  justify-content: center;
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
  margin-right: auto;
  flex: 1;

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

const $ThumbsContainer = styled.div`
  display: flex;
  width: 50px;
  justify-content: space-around;
  align-items: center;
`;
const NowPlaying = ({ user, partyCode, dispatch, connection, currentSong, songFeelings }) => {
  return (
    <$NowPlaying>
      <$NowPlayingSong>
        {currentSong && (
          <React.Fragment>
            <img src={currentSong?.albumImageUrl} />
            <div className="song-information">
              <p>{currentSong?.name}</p>
              <p>{currentSong?.artist}</p>
            </div>

            {songFeelings && currentSong && (
              <$ThumbsContainer>
                <ThumbsDown
                  onDislike={() => userDislikesSong(partyCode, currentSong.uri, connection, dispatch)}
                  feeling={songFeelings[currentSong?.uri]}
                />
                <ThumbsUp onLike={() => userLikesSong(partyCode, currentSong.uri, connection, dispatch)} feeling={songFeelings[currentSong?.uri]} />
              </$ThumbsContainer>
            )}
          </React.Fragment>
        )}
        {!currentSong && <Loader width={50} height={50}></Loader>}
      </$NowPlayingSong>
      <$SongManagement>
        {user?.details?.pausedMusic ? (
          <$PlayFontAwesomeIcon icon={faPlayCircle} onClick={() => togglePlaybackState(partyCode, dispatch)} />
        ) : (
          <$PlayFontAwesomeIcon icon={faPauseCircle} onClick={() => togglePlaybackState(partyCode, dispatch)} />
        )}
        <$SkipFontAwesomeIcon icon={faStepForward} onClick={() => skipSong(partyCode, connection)}></$SkipFontAwesomeIcon>
      </$SongManagement>
      <div style={{ marginRight: "auto", flex: "1" }}></div>
    </$NowPlaying>
  );
};

const mapStateToProps = (state) => {
  return {
    user: getUser(state),
    partyCode: getPartyCode(state),
    connection: getRealtimeConnection(state).connection,
    currentSong: getCurrentSong(state),
    songFeelings: getSongFeelings(state),
  };
};

export default connect(mapStateToProps, null)(NowPlaying);
