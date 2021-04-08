export const setUpSpotifyWebPlayback = (connection) => {
  window.onSpotifyWebPlaybackSDKReady = () => {
    const player = new Spotify.Player({
      name: "Spotibro",
      getOAuthToken: (cb) => {
        fetch("/api/user/GetPartyGoerSpotifyAccessToken")
          .then((json) => json.json())
          .then((res) => cb(res.accessToken));
      },
    });

    // Error handling
    player.addListener("initialization_error", ({ message }) => {
      console.error(message);
    });
    player.addListener("authentication_error", ({ message }) => {
      console.error(message);
    });
    player.addListener("account_error", ({ message }) => {
      console.error(message);
    });
    player.addListener("playback_error", ({ message }) => {
      console.error(message);
    });

    // Playback status updates
    player.addListener("player_state_changed", (state) => {
      console.log("player state changed");
      console.log(state);
    });

    // Ready
    player.addListener("ready", ({ device_id }) => {
      connection.invoke("WebPlayerInitialized", device_id);
    });

    // Not Ready
    player.addListener("not_ready", ({ device_id }) => {});

    // Connect to the player!
    player.connect();
  };
};
