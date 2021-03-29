import React from "react";
import styled from "styled-components";
import { searchSpotify } from "../../../api/party";
import { connect } from "react-redux";

export const $SearchInput = styled.input`
  border: none;
  text-decoration: none;
  background-color: #e0e0e0;
  color: black;
  padding: 10px;
  width: 100%;
  box-sizing: border-box;
  border-radius: 10px;
}
`;

const onSearch = (event, dispatch, setIsLoading) => {
  if (event.target.value != "") {
    setIsLoading(true);
    searchSpotify(event.target.value, dispatch);
  }
};

const SearchInput = (props) => {
  return <$SearchInput placeholder={props.placeholder} onInput={(event) => onSearch(event, props.dispatch, props.setIsLoading)}></$SearchInput>;
};

export default connect(null, null)(SearchInput);
