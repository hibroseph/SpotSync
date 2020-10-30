/** @jsx jsx */
import { jsx } from "theme-ui";
import PrimaryContainer from "./PrimaryContainer";
import Input from "./Input";
import Heading from "./Heading";
import Button from "./Button";
import SuggestedPlaylist from "./SuggestedPlaylist";

export default (props) => {
  return (
    <PrimaryContainer sx={{ width: ["99%", "60%"], flexDirection: "column" }}>
      <Heading>Create a Party</Heading>
      <Input placeholder="Search Spotify for Songs"></Input>
      <SuggestedPlaylist></SuggestedPlaylist>
      <div
        sx={{
          display: "flex",
          flexDirection: "row",
          justifyContent: "flex-end",
        }}
      >
        <Button sx={{ mt: "5px" }}>Create</Button>
      </div>
    </PrimaryContainer>
  );
};
