import React from "react";
import styled from "styled-components";

const ButtonStyle = styled.button`
  height: 50px;
  border-radius: 10px;
  background-color: #e5e5e5;
  border: none;
  font-weight: bold;
  padding: 10px;
  &:hover {
    background-color: #e0e0e0;
  }
`;

const Button = (props) => {
  return <ButtonStyle href={props.link}>{props.title}</ButtonStyle>;
};

export { Button };
