import React from "react";
import styled from "styled-components";
import Voting from "./Voting";
import ArtistLink from "../../shared/ArtistLink";
const $QueueItem = styled.div`
  width: 100%;
  padding: 15px 20px;
  margin: 5px 0px;
  border-radius: 10px;
  display: flex;
  justify-content: space-between;
  box-sizing: border-box;

  &:hover {
    background-color: #fdf8ed;
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

const $centeredQueueItem = styled.div`
  display: flex;
  flex-direction: column;
  justify-content: center;
`;

const $VotingContainer = styled.div`
  display: flex;
`;
const QueueItem = ({ feeling, trackVotes, onLike, onDislike, artists, title }) => {
  return (
    <$QueueItem>
      <$centeredQueueItem>
        <p className="title">{title}</p>
        <div>
          {artists.map((artist, index) => (
            <ArtistLink key={`${artist.id}_${index}`} artist={artist}></ArtistLink>
          ))}
        </div>
      </$centeredQueueItem>
      {feeling != undefined && <Voting feeling={feeling} trackVotes={trackVotes} onDislike={onDislike} onLike={onLike}></Voting>}
    </$QueueItem>
  );
};

export default QueueItem;
