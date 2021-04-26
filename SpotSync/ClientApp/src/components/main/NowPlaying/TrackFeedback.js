import React, { useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faHeart } from "@fortawesome/free-regular-svg-icons";
import { faHeart as fasHeart } from "@fortawesome/free-solid-svg-icons";
import styled from "styled-components";

const $HoverableIcon = styled(FontAwesomeIcon)`
  &:hover {
    transform: scale(1.2);
  }
`;

export default ({ trackId, isFavorite, unfavoriteTrack, favoriteTrack }) => {
  return (
    <div>
      {isFavorite ? (
        <$HoverableIcon icon={fasHeart} onClick={() => unfavoriteTrack(trackId)}></$HoverableIcon>
      ) : (
        <$HoverableIcon icon={faHeart} onClick={() => favoriteTrack(trackId)}></$HoverableIcon>
      )}
    </div>
  );
};
