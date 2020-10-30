/** @jsx jsx */
import { jsx } from "theme-ui";
import React from "react";
import Heading from "./Heading";
import AlbumScroll from "./AlbumScroll.jsx";
import PrimaryContainer from "./PrimaryContainer";

export default (props) => {
  return (
    <PrimaryContainer sx={{ width: ["99%", "80%", "70%", "60%"], flexDirection: "column" }}>
      <Heading>Available Parties</Heading>
      <AlbumScroll></AlbumScroll>
    </PrimaryContainer>
  );
};
