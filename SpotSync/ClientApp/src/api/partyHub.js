export const connectToParty = (partyCode, connection) => {
  console.log("invoking connect to party");
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

export const userLikesSong = (partyCode, trackUri, connection) => {
  connection.invoke("LikeSong", partyCode, trackUri);
};

export const userDislikesSong = (partyCode, trackUri, connection) => {
  connection.invoke("DislikeSong", partyCode, trackUri);
};
