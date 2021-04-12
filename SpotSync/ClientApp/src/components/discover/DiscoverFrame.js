import React, { useState, useEffect } from "react";
import styled from "styled-components";
import Search from "../discover/Search/Search";
import Tabs from "../sidebar/Tabs";
import { addSongToQueue } from "../../api/party";
import { addSomeTracksToQueue as addSomeTracksFromPlaylistToQueue } from "../../api/partyHub";
import { getPlaylists, getTopSongs, getPlaylistItems } from "../../api/user";
import SearchResults from "./Search/SearchResults";
import { getUser, getPartyCode, getRealtimeConnection } from "../../redux/reducers/reducers";
import { connect } from "react-redux";
import PlaylistView from "./Playlists/PlaylistView";
import notify, { error } from "../../api/notify";
import ScrollContainer from "../shared/ScrollContainer";

const $DiscoverFrame = styled.div`
  padding: 0 10px;
  width: 75%;
  padding: 0 0 20px 10px;
  display: flex;
  flex-direction: column;
`;

const $Bar = styled.div`
  display: flex;
  align-items: center;
`;

const addTrackToQueue = (track, user, partyCode, connection) => {
  addSongToQueue(track, user.details.id, partyCode, connection);
};

const viewPlaylist = (playlist, setPlaylistTracks, setPlaylistLoading) => {
  setPlaylistLoading(true);
  getPlaylistItems(playlist.id)
    .then((playlistTracks) => {
      setPlaylistTracks({ tracks: playlistTracks, playlist });
    })
    .catch((err) => error("Unable to load tracks for your playlist. Try again."))
    .finally(() => setPlaylistLoading(false));
};

const addSomeTracksToQueue = (id, amount, connection) => {
  addSomeTracksFromPlaylistToQueue(id, amount, connection);
  notify("Added some tracks from your playlist to the queue");
};

const addSearchResultsToTabs = (tabs, setTabs) => {
  setTabs([...tabs, { title: "Search Results" }]);
};

const DiscoverFrame = ({ user, partyCode, connection }) => {
  const [tabs, setTabs] = useState([
    {
      title: "Your Top Songs",
    },
    {
      title: "Playlists",
    },
  ]);

  const [currentTabView, setTabView] = useState("Your Top Songs");

  const [isLoading, setIsLoading] = useState(false);

  const [searchResults, setSearchResults] = useState([]);
  const [searchTerm, setSearchTerm] = useState(null);
  const [haveTopSongs, setHaveTopSongs] = useState(false);
  const [topSongs, setTopSongs] = useState({});

  const [playlists, setPlaylists] = useState([]);
  const [playlistsLoading, setPlaylistsLoading] = useState(false);
  const [havePlaylists, setHavePlaylists] = useState(false);

  const [playlistTracks, setPlaylistTracks] = useState([]);

  useEffect(() => {
    setIsLoading(false);
  }, [searchResults]);

  useEffect(() => {
    if (currentTabView == "Your Top Songs" && !haveTopSongs && user) {
      getTopSongs(20)
        .then((songs) => {
          setTopSongs(songs);
        })
        .catch((err) => {
          error("There was an error getting your top songs. Try again.");
        })
        .finally(() => {
          setHaveTopSongs(true);
        });
    }

    if (currentTabView == "Playlists" && !havePlaylists) {
      setPlaylistsLoading(true);
      getPlaylists(30, 0)
        .then((playlists) => {
          setPlaylists(playlists);
        })
        .catch((err) => {
          error("There was an error getting your playlists. Try again.");
        })
        .finally(() => {
          setPlaylistsLoading(false);
          setHavePlaylists(true);
        });
    }
  }, [currentTabView, user]);

  return (
    <$DiscoverFrame>
      <$Bar>
        <Search
          addSearchResultsToTabs={() => addSearchResultsToTabs(tabs, setTabs)}
          inputSelected={() => setTabView("Search Results")}
          setIsLoading={setIsLoading}
          setSearchResults={setSearchResults}
        />
        <Tabs selected={currentTabView} changeSelectedTab={setTabView} tabs={tabs} />
      </$Bar>
      <ScrollContainer>
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
            addToQueue={(track) => addTrackToQueue(track, user, partyCode, connection)}
            viewPlaylist={(playlist) => viewPlaylist(playlist, setPlaylistTracks, setPlaylistsLoading)}
            playlistTracks={playlistTracks}
            isLoading={playlistsLoading}
          ></PlaylistView>
        )}
      </ScrollContainer>
    </$DiscoverFrame>
  );
};

const mapStateToProps = (state) => {
  return { user: getUser(state), partyCode: getPartyCode(state), connection: getRealtimeConnection(state).connection };
};
export default connect(mapStateToProps, null)(DiscoverFrame);
