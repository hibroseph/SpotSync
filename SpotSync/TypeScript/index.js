"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var signalR = require("@microsoft/signalr");
var u = require("umbrellajs");
module.exports = {
    RealtimeFunctionality: function ConnectToParty(partyCode) {
    },
    CheckForActiveSpotifyConnection: function CheckForActiveSpotifyConnection() {
        //console.log("Checking for active spotify connection");
        fetch("/api/user/CheckSpotifyForConnection")
            .then(function (response) {
            return response.json();
        }).then(function (device) {
            //console.log(device)
            if (device.deviceName != null) {
                u(".modal").removeClass("is-active");
                // The user has a device that is connected, we need to update him with where we are in the song
                fetch("/party/UpdateSongForUser")
                    .then(function (response) {
                    if (response.status != 200) {
                        //console.log("There was an issue with syncing the music for the user");
                    }
                });
            }
        });
    },
    SyncMusicForUser: function SyncMusicForUser() {
        fetch("/party/UpdateSongForUser")
            .then(function (response) {
            if (response.status != 200) {
                //console.log("There was an issue with syncing the music for the user");
            }
        });
    },
    CloseModal: function CloseModal() {
        u(".modal").removeClass("is-active");
    }
};
//# sourceMappingURL=index.js.map