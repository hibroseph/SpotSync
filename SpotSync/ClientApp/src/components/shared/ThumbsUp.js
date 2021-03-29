import React from "react";
import styled from "styled-components";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faThumbsUp as fasThumbsUp } from "@fortawesome/free-solid-svg-icons";
import { faThumbsUp } from "@fortawesome/free-regular-svg-icons";

const $StyledFontAwesomeIcon = styled(FontAwesomeIcon)`
  &:hover {
    transform: scale(1.2);
  }
`;

export default (props) => (
  <$StyledFontAwesomeIcon onClick={() => props.onLike()} icon={props?.feeling == 1 ? fasThumbsUp : faThumbsUp}></$StyledFontAwesomeIcon>
);
