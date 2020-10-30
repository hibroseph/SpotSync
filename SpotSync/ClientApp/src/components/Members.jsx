/** @jsx jsx */
import { jsx } from "theme-ui";
import React from "react";
import Heading from "./Heading";
import Image from "./Image";
import PrimaryContainer from "./PrimaryContainer";

const members = [
  { id: "iwillridgley", name: "iwillridgley" },
  { id: "hellojordan", name: "hellojordan" },
  { id: "ghostbay", name: "ghostbay" },
  { id: "whimper145", name: "whimper145" },
];

export default (props) => {
  return (
    <PrimaryContainer sx={{ flexDirection: "column" }}>
      <Heading>Members</Heading>
      {members.map((member) => {
        return <p key={member.id}>{member.name}</p>;
      })}
    </PrimaryContainer>
  );
};
