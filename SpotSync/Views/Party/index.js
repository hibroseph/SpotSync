"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var RealTimeConnectionManager_1 = require("../../TypeScript/RealTimeConnectionManager");
var NowPlayingManager_1 = require("../../TypeScript/NowPlayingManager");
var PartyTabManager_1 = require("./PartyTabManager");
var PlaylistManager_1 = require("../../TypeScript/PlaylistManager");
var SearchContainerManager_1 = require("../../TypeScript/SearchContainerManager");
var connectionManager;
var nowPlayingManager;
var partyTabManager;
var playlistManager;
var searchContainerManager;
function ConnectToParty(partyCode) {
    connectionManager = new RealTimeConnectionManager_1.RealTimeConnectionManager();
    connectionManager.connectToParty(partyCode);
    partyTabManager = new PartyTabManager_1.PartyTabManager();
    nowPlayingManager = new NowPlayingManager_1.NowPlayingManager(connectionManager.getConnection(), partyCode);
    playlistManager = new PlaylistManager_1.PlaylistManager(connectionManager.getConnection(), partyCode);
    searchContainerManager = new SearchContainerManager_1.SearchContainerManager(connectionManager.getConnection(), partyCode);
}
exports.ConnectToParty = ConnectToParty;
function SetUpSpotifyPlayback() {
    window.onSpotifyWebPlaybackSDKReady = function () {
        fetch("api/user/GetPartyGoerSpotifyAccessToken")
            .then(function (response) { return response.json(); })
            .then(function (data) {
            console.log("Your api key is " + data.accessToken);
            console.log(data);
            var player = new Spotify.Player({
                name: 'Spotibro',
                getOAuthToken: function (cb) { cb(data.accessToken); }
            });
            // Error handling
            player.addListener('initialization_error', function (_a) {
                var message = _a.message;
                console.error(message);
            });
            player.addListener('authentication_error', function (_a) {
                var message = _a.message;
                console.error(message);
            });
            player.addListener('account_error', function (_a) {
                var message = _a.message;
                console.error(message);
            });
            player.addListener('playback_error', function (_a) {
                var message = _a.message;
                console.error(message);
            });
            // Playback status updates
            player.addListener('player_state_changed', function (state) { console.log(state); });
            // Ready
            player.addListener('ready', function (_a) {
                var device_id = _a.device_id;
                console.log('Ready with Device ID', device_id);
                connectionManager.webPlayerInitialized(device_id);
            });
            // Not Ready
            player.addListener('not_ready', function (_a) {
                var device_id = _a.device_id;
                console.log('Device ID has gone offline', device_id);
            });
            // Connect to the player!
            player.connect();
        });
    };
}
exports.SetUpSpotifyPlayback = SetUpSpotifyPlayback;
//# sourceMappingURL=index.js.map