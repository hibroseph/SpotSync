import React, { useState, useEffect } from "react";
import styled from "styled-components";
import Search from "../discover/Search/Search";
import Tabs from "../sidebar/Tabs";
import { addSongToQueue } from "../../api/party";
import { addSomeTracksToQueue as addSomeTracksFromPlaylistToQueue } from "../../api/partyHub";
import { getPlaylists, getTopSongs, getPlaylistItems } from "../../api/user";
import SearchResults from "./Search/SearchResults";
import { getUser, getPartyCode, getRealtimeConnection, artistView } from "../../redux/reducers/reducers";
import { connect } from "react-redux";
import PlaylistView from "./Playlists/PlaylistView";
import notify, { error } from "../../api/notify";
import ScrollContainer from "../shared/ScrollContainer";
import ArtistView from "./Artists";

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

const SEARCH_RESULTS_TITLE = "Search Results";
const ARTIST_TITLE = "Artist";
const PLAYLIST_TITLE = "Playlist";
const TOP_SONGS_TITLE = "Your Top Songs";

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
  if (tabs.findIndex((p) => p.title == "Search Results")) {
    setTabView(SEARCH_RESULTS_TITLE);
  } else {
    setTabs([...tabs, { title: "Search Results" }]);
  }
};

const DiscoverFrame = ({ user, partyCode, connection, searchArtistId }) => {
  const [tabs, setTabs] = useState([
    {
      title: TOP_SONGS_TITLE,
    },
    {
      title: PLAYLIST_TITLE,
    },
  ]);

  const [currentTabView, setTabView] = useState(TOP_SONGS_TITLE);

  const [isLoading, setIsLoading] = useState(false);

  const [searchResults, setSearchResults] = useState([]);
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
    if (searchArtistId != undefined) {
      setTabView(ARTIST_TITLE);
    }
  }, [searchArtistId]);

  useEffect(() => {
    if (currentTabView == TOP_SONGS_TITLE && !haveTopSongs && user) {
      getTopSongs(20)
        .then((songs) => {
          console.log("top songs");
          console.log(songs);
          setTopSongs(songs);
        })
        .catch((err) => {
          error("There was an error getting your top songs. Try again.");
        })
        .finally(() => {
          setHaveTopSongs(true);
        });
    }

    if (currentTabView == PLAYLIST_TITLE && !havePlaylists) {
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
          inputSelected={() => setTabView(SEARCH_RESULTS_TITLE)}
          setIsLoading={setIsLoading}
          setSearchResults={setSearchResults}
        />
        <Tabs selected={currentTabView} changeSelectedTab={setTabView} tabs={tabs} />
      </$Bar>
      <ScrollContainer>
        {currentTabView == SEARCH_RESULTS_TITLE && (
          <SearchResults
            searchResults={searchResults}
            addSongToQueue={(track) => addTrackToQueue(track, user, partyCode, connection)}
            isLoading={isLoading}
          ></SearchResults>
        )}
        {currentTabView == TOP_SONGS_TITLE && (
          <SearchResults
            searchResults={topSongs}
            addSongToQueue={(track) => addTrackToQueue(track, user, partyCode, connection)}
            isLoading={!haveTopSongs}
          ></SearchResults>
        )}
        {currentTabView == PLAYLIST_TITLE && (
          <PlaylistView
            playlists={playlists}
            addSomeTracksToQueue={(id, amount) => addSomeTracksToQueue(id, amount, connection)}
            addToQueue={(track) => addTrackToQueue(track, user, partyCode, connection)}
            viewPlaylist={(playlist) => viewPlaylist(playlist, setPlaylistTracks, setPlaylistsLoading)}
            playlistTracks={playlistTracks}
            isLoading={playlistsLoading}
          ></PlaylistView>
        )}
        {currentTabView == ARTIST_TITLE && (
          <ArtistView artistId={searchArtistId} addTrackToQueue={(track) => addTrackToQueue(track, user, partyCode, connection)} />
        )}
      </ScrollContainer>
    </$DiscoverFrame>
  );
};

const mapStateToProps = (state) => {
  return {
    user: getUser(state),
    partyCode: getPartyCode(state),
    connection: getRealtimeConnection(state).connection,
    searchArtistId: artistView(state),
  };
};
export default connect(mapStateToProps, null)(DiscoverFrame);
