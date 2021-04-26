import React, { useEffect, useState } from "react";
import styled from "styled-components";
import CenteredHorizontally from "../shared/CenteredHorizontally";
import Loader from "../shared/Loader";
import { searchArtist } from "../../api/browse";
import TrackList from "./TrackList";

const $ArtistImage = styled.img`
  width: 150px;
  height: 150px;
  object-fit: cover;
  border-radius: 100px;
`;

const $ArtistTitle = styled.p`
  font-size: 25px;
  font-weight: bold;
`;
const $TrackListTitle = styled.p`
  font-size: 15px;
  font-weight: bold;
`;
export default ({ artistId, addTrackToQueue }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [currentArtist, setCurrentArtist] = useState(undefined);

  useEffect(() => {
    if (currentArtist == undefined || currentArtist.id != artistId) {
      setIsLoading(true);
      setCurrentArtist(undefined);
      searchArtist(artistId).then((artist) => {
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
          <$ArtistTitle>{currentArtist?.artist.name}</$ArtistTitle>
          {currentArtist?.artist?.images?.length > 0 && <$ArtistImage src={currentArtist?.artist?.images[0].url}></$ArtistImage>}
          <$TrackListTitle>Top Tracks</$TrackListTitle>
          <TrackList tracks={currentArtist?.topTracks} addToQueue={addTrackToQueue}></TrackList>
        </React.Fragment>
      )}
    </div>
  );
};
