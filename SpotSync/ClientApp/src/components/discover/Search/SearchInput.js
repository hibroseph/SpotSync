import React from "react";
import styled from "styled-components";
import { searchSpotify } from "../../../api/party";
import { useEffect, useState } from "react";
import Input from "../../shared/Input";

export const $SearchInput = styled(Input)`
  width: 100%;
`;

const onSearch = (searchValue, setSearchResults) => {
  searchSpotify(searchValue).then((searchResults) => {
    setSearchResults(searchResults);
  });
};

const SearchInput = ({ setIsLoading, placeholder, setSearchResults }) => {
  const [searchValue, setSearchValue] = useState("");

  useEffect(() => {
    if (searchValue == "") {
      setIsLoading(false);
      return;
    }
    setIsLoading(true);
    const timeoutId = setTimeout(() => onSearch(searchValue, setSearchResults), 1000);
    return () => clearTimeout(timeoutId);
  }, [searchValue]);

  return <$SearchInput placeholder={placeholder} onInput={(event) => setSearchValue(event.target.value)}></$SearchInput>;
};

export default SearchInput;
