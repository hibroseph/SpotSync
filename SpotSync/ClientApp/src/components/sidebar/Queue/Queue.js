import React from "react";
import QueueItem from "./QueueItem";

// 0 means downvote, 1 means upvote
const currentQueue = [
  { title: "This Feeling", artist: "The Chainsmokers", vote: 0 },
  { title: "Back Together", artist: "Loote" },
  { title: "COULD HAVE BEEN YOU", artist: "Jake Miller" },
  { title: "Demons", artist: "Wingtip", vote: 1 },
  { title: "Girl in the Mirror", artist: "Bebe Rexha" },
  { title: "Walk Me Home", artist: "P!nk" },
  { title: "Losing It Over You", artist: "Matoma" },
];

const Queue = (props) => {
  return (
    <React.Fragment>
      {currentQueue.map((item) => {
        return <QueueItem {...item}></QueueItem>;
      })}
    </React.Fragment>
  );
};

export default Queue;
