/** @jsx jsx */
import { jsx } from "theme-ui";
import React from "react";
import Heading from "./Heading";
import Image from "./Image";
import PrimaryContainer from "./PrimaryContainer";

const nowPlaying = {
  imageUrl: "https://i.pinimg.com/originals/b4/75/00/b4750046d94fed05d00dd849aa5f0ab7.jpg",
  artist: "Daylily",
  track: "Movements",
  album: "CAME UP FOR AIR",
};

export default (props) => {
  return (
    <PrimaryContainer sx={{ flexDirection: "Column" }}>
      <Heading>Now Playing</Heading>
      <div sx={{ display: "flex", flexDirection: "row", p: "10px" }}>
        <Image imgUrl={nowPlaying.imageUrl}></Image>
        <div sx={{ display: "flex", flexDirection: "column", ml: "40px", justifyContent: "center" }}>
          <b sx={{ py: "10px" }}>{nowPlaying.artist}</b>
          <b sx={{ py: "10px" }}>{nowPlaying.track}</b>
          <b sx={{ py: "10px" }}>{nowPlaying.album}</b>
        </div>
      </div>
    </PrimaryContainer>
  );
};
