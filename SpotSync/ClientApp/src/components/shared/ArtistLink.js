import React from "react";
import styled from "styled-components";

const $ArtistLink = styled.a`
  margin-right: 10px;
  cursor: pointer;

  &:hover {
    text-decoration: underline;
  }
`;

export default ({ artist, ShowArtistView }) => {
  return (
    <$ArtistLink
      onClick={(event) => {
        event.preventDefault();
        ShowArtistView(artist.id);
      }}
    >
      {artist.name}
    </$ArtistLink>
  );
};
