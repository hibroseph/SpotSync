import React from "react";
import styled from "styled-components";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faThumbsUp as fasThumbsUp, faThumbsDown as fasThumbsDown } from "@fortawesome/free-solid-svg-icons";
import { faThumbsUp, faThumbsDown } from "@fortawesome/free-regular-svg-icons";

const $Voting = styled.div`
  width: 50px;
  display: flex;
  justify-content: space-around;
  align-items: center;
`;

const Voting = (props) => {
  const DeterminedVotingIcon = (type) => {};

  const DetermineVotingIcons = () => {
    return (
      <React.Fragment>
        <FontAwesomeIcon icon={props?.vote == 1 ? fasThumbsUp : faThumbsUp}></FontAwesomeIcon>
        <FontAwesomeIcon icon={props?.vote == 0 ? fasThumbsDown : faThumbsDown}></FontAwesomeIcon>
      </React.Fragment>
    );
  };
  return <$Voting>{DetermineVotingIcons()}</$Voting>;
};

export default Voting;
