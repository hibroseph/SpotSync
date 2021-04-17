import { userLikesSong as userLikesSongAction, userDislikesSong as userDislikesSongAction } from "../redux/actions/party";

export const connectToParty = (partyCode, connection) => {
  connection.invoke("ConnectToParty", partyCode);
};

export const skipSong = (partyCode, connection) => {
  connection.invoke("UserWantsToSkipSong", partyCode);
};

export const userAddSongToQueue = (song, user, partyCode, connection) => {
  console.log("hello world");
  console.log(song);
  connection.invoke("UserAddedSong", {
    Name: song.name,
    Artists: song.artists,
    TrackUri: song.id,
    Length: song.duration_ms,
    PartyCode: partyCode,
    IndexToInsertSongAt: 0,
    AddedBy: user,
    Explicit: song.explicit,
  });
};

export const userLikesSong = (partyCode, trackUri, connection, dispatch) => {
  dispatch(userLikesSongAction(trackUri));
  connection.invoke("LikeSong", partyCode, trackUri);
};

export const userDislikesSong = (partyCode, trackUri, connection, dispatch) => {
  dispatch(userDislikesSongAction(trackUri));
  connection.invoke("DislikeSong", partyCode, trackUri);
};

export const addSomeTracksToQueue = (playlistId, amount, connection) => {
  connection.invoke("AddSomeTracksFromPlaylistToQueue", "", playlistId, amount);
};
