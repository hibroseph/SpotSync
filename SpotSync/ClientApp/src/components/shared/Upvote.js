import React from "react";
import styled from "styled-components";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCaretUp } from "@fortawesome/free-solid-svg-icons";

const $StyledFontAwesomeIcon = styled(FontAwesomeIcon)`
  &:hover {
    transform: scale(1.2);
  }
  font-size: 20px;
  cursor: pointer;
  color: ${(p) => (p.selected ? "#7d8aff" : null)};
`;

export default (props) => (
  <$StyledFontAwesomeIcon onClick={() => props.onLike()} selected={props?.feeling == 1} icon={faCaretUp}></$StyledFontAwesomeIcon>
);
