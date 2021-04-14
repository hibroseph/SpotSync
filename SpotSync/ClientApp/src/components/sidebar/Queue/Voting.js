import React from "react";
import styled from "styled-components";
import ThumbsUp from "../../shared/ThumbsUp";
import ThumbsDown from "../../shared/ThumbsDown";

const $Voting = styled.div`
  width: 50px;
  display: flex;
  justify-content: space-around;
  align-items: center;
`;

const Voting = ({ onLike, onDislike, feeling }) => {
  const DetermineVotingIcons = () => {
    return (
      <React.Fragment>
        <ThumbsUp onLike={onLike} feeling={feeling} />
        <ThumbsDown onDislike={onDislike} feeling={feeling} />
      </React.Fragment>
    );
  };
  return <$Voting>{DetermineVotingIcons()}</$Voting>;
};

export default Voting;
