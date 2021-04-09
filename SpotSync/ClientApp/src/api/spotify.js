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
    player.addListener("player_state_changed", (state) => {});

    // Ready
    player.addListener("ready", ({ device_id }) => {
      connection.invoke("WebPlayerInitialized", device_id);
    });

    // Not Ready
    player.addListener("not_ready", ({ device_id }) => {});

    var slider;

    let timeoutId = setInterval(() => {
      if (document.getElementById("spotify-volume-slider") != undefined) {
        slider = document.getElementById("spotify-volume-slider");
        slider.oninput = (value) => {
          const volume = parseFloat(value.target.value);
          player.setVolume(volume == 0 ? 0.0001 : volume * 0.1).catch((err) => console.error(err));
        };

        clearInterval(timeoutId);
      }
    }, 2000);
    // Connect to the player!
    player.connect();
  };
};
