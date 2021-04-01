import { userLikesSong as userLikesSongAction, userDislikesSong as userDislikesSongAction } from "../redux/actions/party";

export const connectToParty = (partyCode, connection) => {
  console.log("invoking connect to party");

  console.log("partycode: " + partyCode);
  console.log(connection);

  connection.invoke("ConnectToParty", partyCode);
};

export const skipSong = (partyCode, connection) => {
  console.log("user is skipping song");
  connection.invoke("UserWantsToSkipSong", partyCode);
};

export const userAddSongToQueue = (song, user, partyCode, connection) => {
  console.log("adding song to queue");
  console.log(song);

  connection.invoke("UserAddedSong", {
    Name: song.name,
    Artist: song.artist,
    TrackUri: song.uri,
    Length: song.length,
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
