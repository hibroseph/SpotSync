import React from "react";
import styled from "styled-components";
import Voting from "./Voting";

const $QueueItem = styled.div`
  width: 100%;
  padding: 10px 8px;
  margin: 5px 0px;
  border-radius: 10px;
  background-color: #f4f4f4;
  display: flex;
  justify-content: space-between;
  box-sizing: border-box;

  p {
    margin: 0px;
    font-size: 10px;
  }

  .title {
    font-weight: bold;
    margin-bottom: 3px;
  }
`;

const $VotingContainer = styled.div`
  display: flex;
`;
const QueueItem = (props) => {
  return (
    <$QueueItem>
      <div>
        <p class="title">{props.title}</p>
        <p>{props.artist}</p>
      </div>
      <Voting vote={props.vote}></Voting>
    </$QueueItem>
  );
};

export default QueueItem;
