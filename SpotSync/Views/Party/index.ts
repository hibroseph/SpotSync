import { connection } from "jquery";
import { RealTimeConnectionManager } from "../../TypeScript/RealTimeConnectionManager";
import { NowPlayingManager } from "../../TypeScript/NowPlayingManager";
import { PartyTabManager } from "../../TypeScript/PartyTabManager";
import { PlaylistManager } from "../../TypeScript/PlaylistManager";
import { SearchContainerManager } from "../../TypeScript/SearchContainerManager";

let connectionManager;
let nowPlayingManager;
let partyTabManager;
let playlistManager;
let searchContainerManager;

export function ConnectToParty(partyCode: string) {

    connectionManager = new RealTimeConnectionManager();
    connectionManager.connectToParty(partyCode);
    partyTabManager = new PartyTabManager()
    nowPlayingManager = new NowPlayingManager(connectionManager.getConnection(), partyCode);
    playlistManager = new PlaylistManager(connectionManager.getConnection(), partyCode);
    searchContainerManager = new SearchContainerManager(connectionManager.getConnection(), partyCode);
}

export function SetUpSpotifyPlayback() {
    window.onSpotifyWebPlaybackSDKReady = () => {

        fetch("api/user/GetPartyGoerSpotifyAccessToken")
            .then(response => response.json())
            .then(data => {
                console.log("Your api key is " + data.accessToken)
                console.log(data);

                const player = new Spotify.Player({
                    name: 'Spotibro',
                    getOAuthToken: cb => { cb(data.accessToken); }
                });

                // Error handling
                player.addListener('initialization_error', ({ message }) => { console.error(message); });
                player.addListener('authentication_error', ({ message }) => { console.error(message); });
                player.addListener('account_error', ({ message }) => { console.error(message); });
                player.addListener('playback_error', ({ message }) => { console.error(message); });

                // Playback status updates
                player.addListener('player_state_changed', state => { console.log(state); });

                // Ready
                player.addListener('ready', ({ device_id }) => {
                    console.log('Ready with Device ID', device_id);
                    connectionManager.webPlayerInitialized(device_id);
                });

                // Not Ready
                player.addListener('not_ready', ({ device_id }) => {
                    console.log('Device ID has gone offline', device_id);
                });

                // Connect to the player!
                player.connect();
            })
    }
}