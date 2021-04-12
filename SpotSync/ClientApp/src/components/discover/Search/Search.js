import React from "react";
import SearchInput from "./SearchInput";
import { searchSpotify } from "../../../api/party";

const onSearch = (searchValue, setSearchResults, addSearchResultsToTabs) => {
  searchSpotify(searchValue).then((searchResults) => {
    setSearchResults(searchResults);
    addSearchResultsToTabs();
  });
};

const Search = ({ addSearchResultsToTabs, inputSelected, setIsLoading, setSearchResults }) => {
  return (
    <SearchInput
      onSearch={(searchValue) => onSearch(searchValue, setSearchResults, addSearchResultsToTabs)}
      setIsLoading={setIsLoading}
      placeholder="Search for Songs, Artists, and Albums"
      inputSelected={inputSelected}
    />
  );
};

export default Search;
