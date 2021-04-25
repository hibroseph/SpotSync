import React from "react";
import styled from "styled-components";
import Upvote from "../../shared/Upvote";
import Downvote from "../../shared/Downvote";

const $Voting = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  color: #b5b5b5;
`;

const Voting = ({ onLike, onDislike, feeling, trackVotes }) => {
  const DetermineVotingIcons = () => {
    return (
      <React.Fragment>
        <Upvote onLike={onLike} feeling={feeling} />
        {trackVotes}
        <Downvote onDislike={onDislike} feeling={feeling} />
      </React.Fragment>
    );
  };
  return <$Voting>{DetermineVotingIcons()}</$Voting>;
};

export default Voting;
