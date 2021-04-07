import React from "react";
import styled from "styled-components";
import Voting from "./Voting";

const $QueueItem = styled.div`
  width: 100%;
  padding: 15px 20px;
  margin: 5px 0px;
  border-radius: 10px;
  display: flex;
  justify-content: space-between;
  box-sizing: border-box;

  &:hover {
    background-color: #e1f1ff;
  }
  p {
    margin: 0px;
    font-size: 15px;
  }

  .title {
    font-weight: bold;
    margin-bottom: 3px;
  }

  .artist {
    font-size: 12px;
    color: #8d8d8d;
  }
`;

const $VotingContainer = styled.div`
  display: flex;
`;
const QueueItem = (props) => {
  return (
    <$QueueItem>
      <div>
        <p className="title">{props.title}</p>
        <p className="artist">{props.artist}</p>
      </div>
      <Voting feeling={props.feeling} onDislike={props.onDislike} onLike={props.onLike} vote={props.vote}></Voting>
    </$QueueItem>
  );
};

export default QueueItem;
