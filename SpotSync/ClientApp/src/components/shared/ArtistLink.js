import React from "react";
import styled from "styled-components";
import { connect } from "react-redux";
import { showArtistView } from "../../redux/actions/views";

const $ArtistLink = styled.a`
  margin-right: 10px;
  cursor: pointer;

  &:hover {
    text-decoration: underline;
  }
`;

const ArtistLink = ({ artist, dispatch }) => {
  return (
    <$ArtistLink
      onClick={(event) => {
        event.preventDefault();
        dispatch(showArtistView(artist.id));
      }}
    >
      {artist.name}
    </$ArtistLink>
  );
};

export default connect()(ArtistLink);
