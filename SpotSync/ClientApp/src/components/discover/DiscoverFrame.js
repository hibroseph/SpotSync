import React, { useState, useEffect } from "react";
import styled from "styled-components";
import Search from "../discover/Search/Search";
import Tabs from "../sidebar/Tabs";
import { addSongToQueue } from "../../api/party";
import { addSomeTracksToQueue as addSomeTracksFromPlaylistToQueue } from "../../api/partyHub";
import { getPlaylists, getTopSongs } from "../../api/user";
import SearchResults from "./Search/SearchResults";
import { getUser, getPartyCode, getRealtimeConnection, getQueue } from "../../redux/reducers/reducers";
import { connect } from "react-redux";
import PlaylistView from "./Playlists/PlaylistView";
import notify from "../../api/notify";
import ScrollContainer from "../shared/ScrollContainer";

const $DiscoverFrame = styled.div`
  padding: 0 10px;
  width: 75%;
  padding: 0 30px 20px 10px;
`;

const $Bar = styled.div`
  display: flex;
  align-items: center;
`;

const tabs = [
  { title: "Search Results" },
  {
    title: "Your Top Songs",
  },
  {
    title: "Playlists",
  },
];

const addTrackToQueue = (track, user, partyCode, connection) => {
  addSongToQueue(track, user.details.id, partyCode, connection);
};

const viewPlaylist = (id) => {
  console.log(`Viewing playlist ${id}`);
};

const addSomeTracksToQueue = (id, amount, connection) => {
  console.log(`Adding ${amount} songs from playlist ${id}`);
  addSomeTracksFromPlaylistToQueue(id, amount, connection);
  notify("Added some tracks from your playlist to the queue");
};

const DiscoverFrame = ({ queue, user, partyCode, connection }) => {
  const [currentTabView, setTabView] = useState("Your Top Songs");

  const [isLoading, setIsLoading] = useState(false);

  const [searchResults, setSearchResults] = useState([]);

  const [haveTopSongs, setHaveTopSongs] = useState(false);
  const [topSongs, setTopSongs] = useState({});

  const [playlists, setPlaylists] = useState([]);
  const [playlistsLoading, setPlaylistsLoading] = useState(false);

  useEffect(() => {
    setIsLoading(false);
  }, [searchResults]);

  useEffect(() => {
    if (currentTabView == "Your Top Songs" && !haveTopSongs && user) {
      getTopSongs(20).then((songs) => {
        setTopSongs(songs);
        setHaveTopSongs(true);
      });
    }

    if (currentTabView == "Playlists") {
      setPlaylistsLoading(true);
      getPlaylists(20, 0)
        .then((playlists) => {
          setPlaylists(playlists);
        })
        .finally(() => {
          setPlaylistsLoading(false);
        });
    }
  }, [currentTabView, user]);

  return (
    <$DiscoverFrame>
      <ScrollContainer>
        <$Bar>
          <Search inputSelected={() => setTabView("Search Results")} setIsLoading={setIsLoading} setSearchResults={setSearchResults} />
          <Tabs selected={currentTabView} changeSelectedTab={setTabView} tabs={tabs} />
        </$Bar>
        {currentTabView == "Search Results" && (
          <SearchResults
            searchResults={searchResults}
            addSongToQueue={(track) => addTrackToQueue(track, user, partyCode, connection)}
            isLoading={isLoading}
          ></SearchResults>
        )}
        {currentTabView == "Your Top Songs" && (
          <SearchResults
            searchResults={topSongs}
            addSongToQueue={(track) => addTrackToQueue(track, user, partyCode, connection)}
            isLoading={!haveTopSongs}
          ></SearchResults>
        )}
        {currentTabView == "Playlists" && (
          <PlaylistView
            playlists={playlists}
            addSomeTracksToQueue={(id, amount) => addSomeTracksToQueue(id, amount, connection)}
            viewPlaylist={(id) => viewPlaylist(id)}
            isLoading={playlistsLoading}
          ></PlaylistView>
        )}
      </ScrollContainer>
    </$DiscoverFrame>
  );
};

const mapStateToProps = (state) => {
  return { user: getUser(state), partyCode: getPartyCode(state), connection: getRealtimeConnection(state).connection, queue: getQueue(state) };
};
export default connect(mapStateToProps, null)(DiscoverFrame);
