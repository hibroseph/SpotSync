export const connectToParty = (partyCode, connection) => {
  console.log("invoking connect to party");
  connection.invoke("ConnectToParty", partyCode);
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
