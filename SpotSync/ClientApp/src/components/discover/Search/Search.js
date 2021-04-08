import React from "react";
import SearchInput from "./SearchInput";
import { searchSpotify } from "../../../api/party";

const onSearch = (searchValue, setSearchResults) => {
  searchSpotify(searchValue).then((searchResults) => {
    setSearchResults(searchResults);
  });
};

const Search = ({ inputSelected, setIsLoading, setSearchResults }) => {
  return (
    <SearchInput
      onSearch={(searchValue) => onSearch(searchValue, setSearchResults)}
      setIsLoading={setIsLoading}
      placeholder="Search for Songs, Artists, and Albums"
      inputSelected={inputSelected}
    />
  );
};

export default Search;
