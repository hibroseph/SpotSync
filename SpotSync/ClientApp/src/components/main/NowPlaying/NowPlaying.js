import React from "react";
import styled from "styled-components";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlayCircle, faStepForward, faPauseCircle } from "@fortawesome/free-solid-svg-icons";
import { connect } from "react-redux";
import { getUser, getPartyCode, getRealtimeConnection } from "../../../redux/reducers/reducers";
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
  .play {
    font-size: 30px;
  }

  .grey-hover {
    &:hover {
      color: grey;
    }
  }

  .skip {
    font-size: 20px;
  }

  .icons {
    display: flex;
    align-items: center;
  }
`;

const NowPlaying = (props) => {
  return (
    <$NowPlaying>
      <div></div>
      <div className="icons">
        {props?.user?.details?.pausedMusic ? (
          <FontAwesomeIcon className="play grey-hover" icon={faPlayCircle} onClick={() => togglePlaybackState(props.partyCode, props.dispatch)} />
        ) : (
          <FontAwesomeIcon className="play grey-hover" icon={faPauseCircle} onClick={() => togglePlaybackState(props.partyCode, props.dispatch)} />
        )}
        <FontAwesomeIcon
          className="skip grey-hover"
          icon={faStepForward}
          onClick={() => skipSong(props.partyCode, props.connection)}
        ></FontAwesomeIcon>
      </div>
      <div></div>
    </$NowPlaying>
  );
};

const mapStateToProps = (state) => {
  return { user: getUser(state), partyCode: getPartyCode(state), connection: getRealtimeConnection(state).connection };
};

export default connect(mapStateToProps, null)(NowPlaying);
