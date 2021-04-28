import React from "react";
import styled from "styled-components";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus } from "@fortawesome/free-solid-svg-icons";
import { addSongToQueue } from "../../../api/party";
import notify from "../../shared/notify";
import ArtistLink from "../../shared/ArtistLink";

const $Track = styled.div`
  border: 3px solid #e1e1e1;
  border-radius: 10px;
  margin: 10px;
  padding: 10px 20px;
  width: 200px;
  font-size: 14px;
  display: flex;
  align-items: center;
  justify-content: space-between;

  &:hover {
    background-color: #fdf8ed;
    border-color: #fdf8ed;
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

const Track = ({ addSongToQueue, track }) => {
  return (
    <$Track>
      <div className="song-information">
        <p className="title">{track.name}</p>
        <div>
          {track.artists.map((artist, index) => (
            <ArtistLink key={`${artist}_${index}`} artist={artist}></ArtistLink>
          ))}
        </div>
      </div>
      <$StyledFontAwesomeIcon icon={faPlus} onClick={addSongToQueue} />
    </$Track>
  );
};

export default Track;
