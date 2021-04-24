import React from "react";
import styled from "styled-components";
import ThumbsUp from "../../shared/ThumbsUp";
import ThumbsDown from "../../shared/ThumbsDown";

const $Voting = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  color: #b5b5b5;
`;

const Voting = ({ onLike, onDislike, feeling, dislikeNumber, likeNumber }) => {
  const DetermineVotingIcons = () => {
    return (
      <React.Fragment>
        <ThumbsUp onLike={onLike} feeling={feeling} />
        {likeNumber - dislikeNumber}
        <ThumbsDown onDislike={onDislike} feeling={feeling} />
      </React.Fragment>
    );
  };
  return <$Voting>{DetermineVotingIcons()}</$Voting>;
};

export default Voting;
