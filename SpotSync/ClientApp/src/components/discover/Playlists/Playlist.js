import React, { useState } from "react";
import styled from "styled-components";
import Text from "../../shared/Text";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faRandom, faListUl } from "@fortawesome/free-solid-svg-icons";
import Image from "../../shared/Image";

const $Image = styled(Image)`
  border-radius: 10px;
  margin-bottom: 10px;
`;

const $PlaylistContainer = styled.div`
  position: relative;
  display: flex;
  flex-direction: column;
  padding: 20px;
  align-items: center;
  width: 200px;
`;

const $PlaylistOptionsContainer = styled.div`
  position: absolute;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 150px;
  height: 150px;
  border-radius: 10px;
  background-color: rgba(0, 0, 0, 0.7);
  backdrop-filter: blur(5px);
`;

const $PlaylistOptions = styled.div`
  display: flex;
  justify-content: space-around;
  width: 80%;
  border-radius: 100px;
  background-color: #7d8aff;
`;

const $StyledIcon = styled(FontAwesomeIcon)`
  color: white;
  font-size: 30px;
  padding: 10px;
  margin: 4px;

  &:hover {
    background-color: rgba(0, 0, 0, 0.5);
    border-radius: 25px;
  }
`;

export default ({ playlist, addSomeTracksToQueue, viewPlaylist }) => {
  const [isHovering, setIsHovering] = useState(false);
  return (
    <$PlaylistContainer>
      <$Image onMouseEnter={() => setIsHovering(true)} src={playlist.playlistCoverArtUrl}></$Image>
      {isHovering && (
        <$PlaylistOptionsContainer onMouseLeave={() => setIsHovering(false)}>
          <$PlaylistOptions>
            <$StyledIcon onClick={() => addSomeTracksToQueue(playlist.id, 5)} icon={faRandom}></$StyledIcon>
            <$StyledIcon onClick={viewPlaylist} icon={faListUl}></$StyledIcon>
          </$PlaylistOptions>
        </$PlaylistOptionsContainer>
      )}
      <Text>{playlist.name}</Text>
    </$PlaylistContainer>
  );
};
