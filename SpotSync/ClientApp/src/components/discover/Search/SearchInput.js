import React from "react";
import styled from "styled-components";
import { searchSpotify } from "../../../api/party";
import { useEffect, useState } from "react";

export const $SearchInput = styled.input`
  border: none;
  text-decoration: none;
  background-color: #e0e0e0;
  color: black;
  padding: 10px;
  width: 100%;
  box-sizing: border-box;
  border-radius: 10px;
  font-size: 15px;
}
`;

const onSearch = (searchValue, setSearchResults) => {
  searchSpotify(searchValue).then((searchResults) => {
    setSearchResults(searchResults);
  });
};

const SearchInput = ({ setSearchTerm, setIsLoading, placeholder, setSearchResults }) => {
  const [searchValue, setSearchValue] = useState("");

  useEffect(() => {
    if (searchValue == "") {
      setIsLoading(false);
      return;
    }
    setSearchTerm(searchValue);
    setIsLoading(true);
    const timeoutId = setTimeout(() => onSearch(searchValue, setSearchResults), 1000);
    return () => clearTimeout(timeoutId);
  }, [searchValue]);

  return <$SearchInput placeholder={placeholder} onInput={(event) => setSearchValue(event.target.value)}></$SearchInput>;
};

export default SearchInput;
