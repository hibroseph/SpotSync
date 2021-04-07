import React from "react";
import styled from "styled-components";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus } from "@fortawesome/free-solid-svg-icons";
import { addSongToQueue } from "../../../api/party";
import notify from "../../../api/notify";

const $Track = styled.div`
  border: 3px solid #fafafa;
  border-radius: 10px;
  margin: 10px;
  padding: 10px 20px;
  width: 200px;
  font-size: 14px;
  display: flex;
  align-items: center;
  justify-content: space-between;

  &:hover {
    background-color: #e1f1ff;
  }

  .title {
    margin: 0px;
    font-weight: bold;
    font-size: 15px;
  }

  .artist {
    font-size: 12px;
    color: grey;
    margin: 0px;
  }
`;

const $StyledFontAwesomeIcon = styled(FontAwesomeIcon)`
  &:hover {
    transform: scale(1.2);
  }
`;

const Track = (props) => {
  return (
    <$Track>
      <div className="song-information">
        <p className="title">{props.track.name}</p>
        <p className="artist">{props.track.artist}</p>
      </div>
      <$StyledFontAwesomeIcon
        icon={faPlus}
        onClick={() => {
          if (props.isValidAddition()) {
            addSongToQueue(props.track, props.user.details.id, props.partyCode, props.connection);
            notify(`We added ${props.track.name} to the queue`);
          } else {
            notify(`${props.track.name} already exists in the queue. Go ahead and add it when it leaves the queue`);
          }
        }}
      />
    </$Track>
  );
};

export default Track;
