import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlayCircle, faStepForward, faPauseCircle } from "@fortawesome/free-solid-svg-icons";
import { connect } from "react-redux";
import { getUser, getPartyCode, getRealtimeConnection, getCurrentSong, isHost, getStartPosition } from "../../../redux/reducers/reducers";
import { togglePlaybackState } from "../../../api/party";
import { skipSong } from "../../../api/partyHub";
import { getFavoriteTracks } from "../../../api/user";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.min.css";
import NoAlbumArt from "../../../assets/unknown-album-art.png";
import Image from "../../shared/Image";
import ArtistLink from "../../shared/ArtistLink";
import MusicProgressBar from "./MusicProgressBar";
import TrackFeedback from "./TrackFeedback";
import { favoriteTrack, unfavoriteTrack } from "../../../api/user";
import { error } from "../../shared/notify";

const $NowPlaying = styled.div`
  box-sizing: border-box;
  width: 100%;
  bottom: 0px;
  left: 0;
  padding: 15px;
  display: flex;
  justify-content: space-around;

  flex: 0 1 30px;
`;

const $SongManagement = styled.div`
  display: flex;
  flex: 1;
  align-items: center;
  justify-content: center;
`;

const $VolumeManagement = styled.div`
  display: flex;
  align-items: center;
  justify-content: center;
  marginright: auto;
  flex: 1;
`;

const $Volume = styled.input``;

const $PlayFontAwesomeIcon = styled(FontAwesomeIcon)`
  font-size: 30px;

  &:hover {
    color: grey;
  }
`;

const $SkipFontAwesomeIcon = styled(FontAwesomeIcon)`
  font-size: 20px;

  &:hover {
    color: grey;
  }
`;

const $NowPlayingSong = styled.div`
  display: flex;
  margin-right: auto;
  flex: 1;
  align-items: center;
  .song-information {
    display: flex;
    flex-direction: column;
    justify-content: center;
    padding: 10px;
  }
  p {
    margin: 0px;
  }

  img {
    width: 50px;
    height: 50px;
  }

  .title {
    font-size: 15px;
    font-weight: bold;
  }

  .artist {
    font-size: 12px;
  }
`;

const $ThumbsContainer = styled.div`
  display: flex;
  width: 50px;
  justify-content: space-around;
  align-items: center;
`;

const NowPlaying = ({ user, partyCode, dispatch, connection, currentSong, isHost, ShowArtistView, startPosition }) => {
  const addAsFavorite = (trackId) => {
    setFavoriteTracks([...favoriteTracks, trackId]);
  };

  const removeAsFavorite = (trackId) => {
    setFavoriteTracks(favoriteTracks.filter((p) => !p.includes(trackId)));
  };

  const [favoriteTracks, setFavoriteTracks] = useState([]);

  useEffect(() => {
    getFavoriteTracks().then((favoriteTracks) => {
      setFavoriteTracks(favoriteTracks);
    });
  }, []);

  return (
    <React.Fragment>
      <ToastContainer
        position="bottom-center"
        style={{ bottom: "100px" }}
        autoClose={5000}
        hideProgressBar={false}
        newestOnTop={false}
        closeOnClick
        rtl={false}
        pauseOnFocusLoss
        draggable
        pauseOnHover
      ></ToastContainer>
      {partyCode && currentSong?.id && (
        <React.Fragment>
          {/*<MusicProgressBar millisecond={startPosition} lengthOfSong={currentSong.length}></MusicProgressBar>*/}
          <$NowPlaying>
            <$NowPlayingSong>
              <React.Fragment>
                <Image src={currentSong?.albumImageUrl != undefined ? currentSong.albumImageUrl : NoAlbumArt} />
                <div className="song-information">
                  <p className={"title"}>{currentSong?.name}</p>
                  <div>
                    {currentSong?.artists?.map((artist, index) => (
                      <ArtistLink key={`${artist.id}_${index}`} ShowArtistView={ShowArtistView} artist={artist}></ArtistLink>
                    ))}
                  </div>
                </div>
                <TrackFeedback
                  trackId={currentSong?.id}
                  isFavorite={favoriteTracks.filter((p) => p.includes(currentSong?.id?.split("+")[0])).length > 0}
                  favoriteTrack={() => {
                    favoriteTrack(currentSong?.id)
                      .then((p) => addAsFavorite(currentSong?.id?.split("+")[0]))
                      .catch((p) => error("Failed to favorite track. Please try again later."));
                  }}
                  unfavoriteTrack={() => {
                    unfavoriteTrack(currentSong?.id)
                      .then((p) => removeAsFavorite(currentSong?.id?.split("+")[0]))
                      .catch((p) => error("Failed to unfavorite track. Please try again later."));
                  }}
                ></TrackFeedback>
              </React.Fragment>
            </$NowPlayingSong>
            <$SongManagement>
              {user?.details?.pausedMusic ? (
                <$PlayFontAwesomeIcon icon={faPlayCircle} onClick={() => togglePlaybackState(partyCode, dispatch)} />
              ) : (
                <$PlayFontAwesomeIcon icon={faPauseCircle} onClick={() => togglePlaybackState(partyCode, dispatch)} />
              )}
              {isHost && <$SkipFontAwesomeIcon icon={faStepForward} onClick={() => skipSong(partyCode, connection)}></$SkipFontAwesomeIcon>}
            </$SongManagement>
            <$VolumeManagement>
              <$Volume type="range" min="0" max="10" id="spotify-volume-slider" />
            </$VolumeManagement>
          </$NowPlaying>
        </React.Fragment>
      )}
    </React.Fragment>
  );
};

const mapStateToProps = (state) => {
  return {
    user: getUser(state),
    partyCode: getPartyCode(state),
    connection: getRealtimeConnection(state).connection,
    currentSong: getCurrentSong(state),
    isHost: isHost(state),
    startPosition: getStartPosition(state),
  };
};

export default connect(mapStateToProps, null)(NowPlaying);
