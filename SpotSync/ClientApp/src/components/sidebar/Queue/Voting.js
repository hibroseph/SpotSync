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

const $StyledFontAwesomeIcon = styled(FontAwesomeIcon)`
  &:hover {
    transform: scale(1.2);
  }
`;

const Voting = (props) => {
  const DeterminedVotingIcon = (type) => {};

  const DetermineVotingIcons = () => {
    return (
      <React.Fragment>
        <$StyledFontAwesomeIcon onClick={() => props.onLike()} icon={props?.feeling == 1 ? fasThumbsUp : faThumbsUp}></$StyledFontAwesomeIcon>
        <$StyledFontAwesomeIcon onClick={() => props.onDislike()} icon={props?.feeling == 0 ? fasThumbsDown : faThumbsDown}></$StyledFontAwesomeIcon>
      </React.Fragment>
    );
  };
  return <$Voting>{DetermineVotingIcons()}</$Voting>;
};

export default Voting;
