import React from "react";
import styled from "styled-components";
import { searchSpotify } from "../../../api/party";
import { useEffect, useState } from "react";
import Input from "../../shared/Input";

export const $SearchInput = styled(Input)`
  width: 40%;
`;

const SearchInput = ({ setIsLoading, placeholder, onSearch, inputSelected }) => {
  const [searchValue, setSearchValue] = useState("");

  useEffect(() => {
    if (searchValue == "") {
      setIsLoading(false);
      return;
    }
    setIsLoading(true);
    const timeoutId = setTimeout(() => onSearch(searchValue), 1000);
    return () => clearTimeout(timeoutId);
  }, [searchValue]);

  return (
    <$SearchInput onFocus={() => inputSelected()} placeholder={placeholder} onInput={(event) => setSearchValue(event.target.value)}></$SearchInput>
  );
};

export default SearchInput;
