import React from "react";
import styled from "styled-components";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus } from "@fortawesome/free-solid-svg-icons";
import { addSongToQueue } from "../../../api/party";

const $Track = styled.div`
  background-color: #e0e0e0;
  border-radius: 10px;
  margin: 5px;
  padding: 5px;
  width: 150px;
  font-size: 12px;
  display: flex;
  align-items: center;
  justify-content: space-between;

  .title {
    margin: 0px;
  }

  .artist {
    color: grey;
    margin: 0px;
  }
`;

const Track = (props) => {
  return (
    <$Track>
      <div className="song-information">
        <p className="title">{props.track.name}</p>
        <p className="artist">{props.track.artist}</p>
      </div>
      <FontAwesomeIcon
        icon={faPlus}
        onClick={() => addSongToQueue(props.track, props.user.details.id, props.partyCode, props.connection)}
      ></FontAwesomeIcon>
    </$Track>
  );
};

export default Track;
