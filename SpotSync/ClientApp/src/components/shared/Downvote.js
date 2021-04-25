import React from "react";
import styled from "styled-components";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCaretDown } from "@fortawesome/free-solid-svg-icons";

const $StyledFontAwesomeIcon = styled(FontAwesomeIcon)`
  &:hover {
    transform: scale(1.2);
  }
  cursor: pointer;
  font-size: 20px;
  color: ${(p) => (p.selected ? "#7d8aff" : null)};
`;

export default (props) => (
  <$StyledFontAwesomeIcon onClick={() => props.onDislike()} selected={props?.feeling == 0} icon={faCaretDown}></$StyledFontAwesomeIcon>
);
