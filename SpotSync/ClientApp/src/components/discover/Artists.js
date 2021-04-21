import React, { useEffect, useState } from "react";
import styled from "styled-components";
import CenteredHorizontally from "../shared/CenteredHorizontally";
import Loader from "../shared/Loader";
import { searchArtist } from "../../api/browse";
import TrackList from "./TrackList";

const $ArtistImage = styled.img`
  width: 20%;
  border-radius: 25px;
`;
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
        setCurrentArtist(artist);
        setIsLoading(false);
      });
    }
  }, [artistId]);

  console.log(currentArtist);
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
          {currentArtist?.artist?.images?.length > 0 && <$ArtistImage src={currentArtist?.artist?.images[0].url}></$ArtistImage>}
          <TrackList tracks={currentArtist?.topTracks} addToQueue={addTrackToQueue}></TrackList>
        </React.Fragment>
      )}
    </div>
  );
};
