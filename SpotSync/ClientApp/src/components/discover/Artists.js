import React, { useEffect, useState } from "react";
import styled from "styled-components";
import CenteredHorizontally from "../shared/CenteredHorizontally";
import Loader from "../shared/Loader";
import { searchArtist } from "../../api/browse";
import { SearchResults } from "./Search/SearchResults";
import TrackList from "./TrackList";

export default ({ artistId, addTrackToQueue }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [currentArtist, setCurrentArtist] = useState(undefined);

  useEffect(() => {
    if (currentArtist == undefined || currentArtist.id != artistId) {
      setIsLoading(true);
      setCurrentArtist(undefined);
      console.log("CALLING BROWSE ARTIST API");
      searchArtist(artistId).then((artist) => {
        console.log("artist returned from api");
        console.log(artist);
        console.log(artist.artist);
        console.log(artist.artist.images.length);
        console.log(artist.artist.images[0].url);
        setCurrentArtist(artist);
        setIsLoading(false);
      });
    }
  }, [artistId]);

  return (
    <div>
      {isLoading && (
        <CenteredHorizontally>
          <Loader></Loader>
        </CenteredHorizontally>
      )}
      {!isLoading && (
        <React.Fragment>
          <p>{currentArtist?.artist.name}</p>
          {currentArtist?.artist?.images?.length > 0 && <img src={currentArtist?.artist?.images[0].url}></img>}
          <TrackList tracks={currentArtist?.topTracks} addToQueue={addTrackToQueue}></TrackList>
        </React.Fragment>
      )}
    </div>
  );
};
