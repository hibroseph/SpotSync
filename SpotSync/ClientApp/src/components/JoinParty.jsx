/** @jsx jsx */
import { jsx } from "theme-ui";
import Heading from "./Heading";
import Button from "./Button";
import Input from "./Input";
import Container from "./PrimaryContainer";

export default (props) => {
  return (
    <Container sx={{ height: "5%", flexDirection: "column", width: ["99%", "40%"] }}>
      <Heading>Join Party</Heading>
      <div sx={{ display: "flex", flexDirection: "row", justifyContent: "space-between" }}>
        <Input sx={{ mr: "5px" }} placeholder="Party Code"></Input>
        <Button>Join</Button>
      </div>
    </Container>
  );
};
