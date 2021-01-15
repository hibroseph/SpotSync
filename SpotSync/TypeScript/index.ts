const signalR = require("@microsoft/signalr")
const u = require("umbrellajs");
import { fromEvent, interval, of } from 'rxjs';
import { debounce, map, catchError, startWith, tap, filter } from 'rxjs/operators';
import { ajax } from 'rxjs/ajax';
import { NowPlayingManager } from './NowPlayingManager';
import { PartyTabManager } from "./PartyTabManager";
module.exports = {
    RealtimeFunctionality: function ConnectToParty(partyCode: string) {


    },
    CheckForActiveSpotifyConnection: function CheckForActiveSpotifyConnection() {
        //console.log("Checking for active spotify connection");
        fetch("/api/user/CheckSpotifyForConnection")
            .then(response =>
                response.json()
            ).then(device => {
                //console.log(device)
                if (device.deviceName != null) {
                    u(".modal").removeClass("is-active")

                    // The user has a device that is connected, we need to update him with where we are in the song
                    fetch("/party/UpdateSongForUser")
                        .then(response => {
                            if (response.status != 200) {
                                //console.log("There was an issue with syncing the music for the user");
                            }
                        });
                }
            })
    },
    SyncMusicForUser: function SyncMusicForUser() {
        fetch("/party/UpdateSongForUser")
            .then(response => {
                if (response.status != 200) {
                    //console.log("There was an issue with syncing the music for the user");
                }
            });
    },
    CloseModal: function CloseModal() {
        u(".modal").removeClass("is-active");
    }
}






