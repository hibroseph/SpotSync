import React from "react";
import styled from "styled-components";
import Text from "../shared/Text";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus } from "@fortawesome/free-solid-svg-icons";
import ArtistLink from "../shared/ArtistLink";

const $TrackListItem = styled.div`
  display: flex;
  padding: 20px;
  border-radius: 10px;

  &:hover {
    background-color: #fdf8ed;
  }
`;

const $FlexText = styled(Text)`
  flex: 1;
`;

const $FlexQueueNumber = styled($FlexText)`
  flex: 0.1;
  filter: opacity(0.3);
`;

const $FlexArtist = styled($FlexText)`
  font-weight: normal;
`;

const $StyledFontAwesomeIcon = styled(FontAwesomeIcon)`
  &:hover {
    transform: scale(1.2);
  }
`;

export default ({ track, index, addToQueue }) => {
  return (
    <$TrackListItem>
      <$FlexQueueNumber>{index}</$FlexQueueNumber>
      <$FlexText>{track.name}</$FlexText>
      <$FlexArtist>
        <div>
          {track.artists.map((artist) => (
            <ArtistLink artist={artist}></ArtistLink>
          ))}
        </div>
      </$FlexArtist>
      <$StyledFontAwesomeIcon icon={faPlus} onClick={addToQueue}></$StyledFontAwesomeIcon>
    </$TrackListItem>
  );
};
