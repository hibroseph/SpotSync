import React from "react";
import styled from "styled-components";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlayCircle, faStepForward, faPause } from "@fortawesome/free-solid-svg-icons";
import { connect } from "react-redux";
import { getUser, getPartyCode } from "../../../redux/reducers/reducers";
import { togglePlaybackState } from "../../../api/party";

const $NowPlaying = styled.div`
  position: absolute;
  box-sizing: border-box;
  width: 100%;
  bottom: 0px;
  left: 0;
  padding: 20px;
  background-color: #e5e5e5;
  display: flex;
  justify-content: space-around;

  .play {
    font-size: 30px;
  }

  .grey-hover {
    &:hover {
      color: grey;
    }
  }

  .skip {
    font-size: 25px;
  }
`;

const NowPlaying = (props) => {
  return (
    <$NowPlaying>
      <div></div>
      <div className="icons">
        {props?.user?.details?.pausedMusic ? (
          <FontAwesomeIcon
            className="play grey-hover"
            icon={faPause}
            onClick={() => togglePlaybackState(props.partyCode, props.dispatch)}
          ></FontAwesomeIcon>
        ) : (
          <FontAwesomeIcon
            className="play grey-hover"
            icon={faPlayCircle}
            onClick={() => togglePlaybackState(props.partyCode, props.dispatch)}
          ></FontAwesomeIcon>
        )}
        <FontAwesomeIcon className="skip grey-hover" icon={faStepForward}></FontAwesomeIcon>
      </div>
      <div></div>
    </$NowPlaying>
  );
};

const mapStateToProps = (state) => {
  return { user: getUser(state), partyCode: getPartyCode(state) };
};

export default connect(mapStateToProps, null)(NowPlaying);
