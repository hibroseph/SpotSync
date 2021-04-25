import { userLikesSong as userLikesSongAction, userDislikesSong as userDislikesSongAction } from "../redux/actions/party";

export const connectToParty = (partyCode, connection) => {
  connection.invoke("ConnectToParty", partyCode);
};

export const skipSong = (partyCode, connection) => {
  connection.invoke("UserWantsToSkipSong", partyCode);
};

export const userAddSongToQueue = (song, user, partyCode, connection) => {
  console.log("ADDING SONG TO QUEUE");
  console.log("hello world");
  console.log(song);
  connection.invoke("UserAddedSong", {
    Name: song.name,
    Artists: song.artists,
    TrackUri: song.id,
    Length: song.duration,
    PartyCode: partyCode,
    IndexToInsertSongAt: 0,
    AddedBy: user,
    Explicit: song.explicit,
  });
};

export const userLikesSong = (partyCode, trackUri, connection, dispatch) => {
  console.log(trackUri);
  dispatch(userLikesSongAction(trackUri));
  connection.invoke("AddTrackFeeling", partyCode, trackUri, 1);
};

export const userDislikesSong = (partyCode, trackUri, connection, dispatch) => {
  dispatch(userDislikesSongAction(trackUri));
  connection.invoke("AddTrackFeeling", partyCode, trackUri, 0);
};

export const addSomeTracksToQueue = (playlistId, amount, connection) => {
  connection.invoke("AddSomeTracksFromPlaylistToQueue", "", playlistId, amount);
};

export const nukeQueue = (partyCode, connection) => {
  connection.invoke("NukeQueue", partyCode);
};
