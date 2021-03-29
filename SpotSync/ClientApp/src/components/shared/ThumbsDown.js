import React from "react";
import styled from "styled-components";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faThumbsDown as fasThumbsDown } from "@fortawesome/free-solid-svg-icons";
import { faThumbsDown } from "@fortawesome/free-regular-svg-icons";

const $StyledFontAwesomeIcon = styled(FontAwesomeIcon)`
  &:hover {
    transform: scale(1.2);
  }
`;

export default (props) => (
  <$StyledFontAwesomeIcon onClick={() => props.onDislike()} icon={props?.feeling == 0 ? fasThumbsDown : faThumbsDown}></$StyledFontAwesomeIcon>
);
