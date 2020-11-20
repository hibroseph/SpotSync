"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var signalR = require("@microsoft/signalr");
var u = require("umbrellajs");
var rxjs_1 = require("rxjs");
var operators_1 = require("rxjs/operators");
var ajax_1 = require("rxjs/ajax");
module.exports = {
    RealtimeFunctionality: function ConnectToParty(partyCode) {
        console.log("Connecting to real time server");
        var connection = new signalR.HubConnectionBuilder().withUrl("/partyhub").build();
        connection.start().then(function () { return connection.invoke("ConnectToParty", partyCode); });
        connection.on("UpdateParty", function (msg) {
            console.log(msg);
        });
        connection.on("UpdatePartyView", function (current, history, queue) {
            UpdateCurrentSong(current.song, current.position);
            UpdateHistory(history);
            UpdateQueue(queue);
            console.log(current);
            console.log(history);
            console.log(queue);
        });
        connection.on("UpdateSong", function (song) {
            UpdateSong(song.song);
            console.log(song);
        });
        connection.on("NewListener", function (userName) {
            console.log(userName);
            u("#listeners").append("<p>" + userName + "</p>");
        });
        connection.on("ConnectSpotify", function (msg) {
            console.log("Users Spotify is disconnected");
            u("#card-content").html("<p>It looks like your Spotify got disconnected from us. This might be because your Spotify app was closed. Make sure your Spotify app is open on whatever device you want to listen through and start playing a song and press the button below to see if we can find your Spotify</p>");
            u(".modal").addClass("is-active");
        });
        rxjs_1.fromEvent(document.getElementById("search-spotify-input"), 'input').subscribe(function (event) {
            u("#results").empty();
            u("#results").siblings("#loader").addClass("is-active");
        });
        u("#is-search-spotify-wrapper").on("mouseleave", function (event) {
            u("#is-search-spotify-wrapper").removeClass("increment-search-box");
            u("#results").addClass("hidden");
        });
        u("#search-spotify-input").on("input", function (event) {
            u("#is-search-spotify-wrapper").addClass("increment-search-box");
            u("#results").removeClass("hidden");
        });
        /*
        u("#results").find("li").on('click', (event) => {
            console.log("Adding song to playlist");
            connection.invoke("UserAddedSong", {
                partyCode: partyCode,
                indexToInsertSongAt: null,
                trackUri: event.target.dataset.trackuri,
                albumImageUrl: event.target.dataset.albumimageurl,
                title: event.target.dataset.title,
                artist: event.target.dataset.artist,
                length: parseInt(event.target.dataset.length)
            });
        })
        */
        rxjs_1.fromEvent(document.getElementById("search-spotify-input"), 'input').pipe(operators_1.debounce(function () { return rxjs_1.interval(1500); })).subscribe(function (event) {
            console.log(event.target.value);
            ajax_1.ajax.getJSON("/api/user/searchSpotify?query=" + event.target.value + "&queryType=0").pipe(operators_1.catchError(function (error) {
                console.log('error: ', error);
                return rxjs_1.of(error);
            })).subscribe(function (response) {
                console.log("Track response");
                // remove loading icon
                u("#loader").removeClass("is-active");
                u("#results").removeClass("hidden");
                response.map(function (song) {
                    u("#results").append("<li tabindex=\"\" data-albumimageurl=" + song.albumImageUrl + " data-title=" + song.title + " data-artist=" + song.artist + " data-length=" + song.length + " data-trackuri=" + song.trackUri + "> <span>" + song.title + " < /span><span class=\"artist\">" + song.artist + "</span > </li>");
                });
            });
            ajax_1.ajax.getJSON("/api/user/searchSpotify?query=" + event.target.value + "&queryType=1").pipe(operators_1.catchError(function (error) {
                console.log('error: ', error);
                return rxjs_1.of(error);
            })).subscribe(function (response) {
                console.log("Artist response");
                // remove loading icon
                u("#loader").removeClass("is-active");
                u("#results").removeClass("hidden");
                response.map(function (song) {
                    u("#results").append("<li tabindex=\"\" data-albumimageurl=" + song.albumImageUrl + " data-title=" + song.title + " data-artist=" + song.artist + " data-length=" + song.length + " data-trackuri=" + song.trackUri + "> <span>" + song.title + " < /span><span class=\"artist\">" + song.artist + "</span > </li>");
                });
            });
            ajax_1.ajax.getJSON("/api/user/searchSpotify?query=" + event.target.value + "&queryType=2").pipe(operators_1.catchError(function (error) {
                console.log('error: ', error);
                return rxjs_1.of(error);
            })).subscribe(function (response) {
                console.log("Album response");
                // remove loading icon
                u("#loader").removeClass("is-active");
                u("#results").removeClass("hidden");
                response.map(function (song) {
                    u("#results").append("<li tabindex=\"\" data-albumimageurl=" + song.albumImageUrl + " data-title=" + song.title + " data-artist=" + song.artist + " data-length=" + song.length + " data-trackuri=" + song.trackUri + "> <span>" + song.title + " < /span><span class=\"artist\">" + song.artist + "</span > </li>");
                });
            });
        });
        function UpdateCurrentSong(song, position) {
            document.getElementById("albumArt").setAttribute("src", song.albumImageUrl);
            document.getElementById("track").innerText = song.title;
            document.getElementById("artist").innerText = song.artist;
        }
        function UpdateHistory(history) {
            u("#history").siblings("#loader").removeClass("is-active");
            u("#history").empty();
            history.map(function (song) {
                u("#history").append("<tr><td>" + song.title + "</td><td>" + song.artist + "</td></tr>");
            });
        }
        function UpdateQueue(queue) {
            u("#queue").siblings("#loader").removeClass("is-active");
            u("#queue").empty();
            console.log("Start of update queue");
            queue.map(function (song) {
                u("#queue").append("<tr><td>" + song.title + "</td><td>" + song.artist + "</td></tr>");
                //u("#queue").append(`<div>${song.title}</div>`)
            });
            MakeFirstQueueElementHidden();
            var element = document.getElementById("queue");
            // @ts-ignore
            var sortable = Sortable.create(element, {
                group: 'shared',
                animation: 300,
                easing: "cubic-bezier(0.76, 0, 0.24, 1)",
                ghostClass: "has-background-white-ter",
                chosenClass: "has-background-info-light",
                dragClass: "has-background-info-light",
                onEnd: function (evt) {
                    console.log(evt);
                    if (evt.oldIndex !== evt.newIndex) {
                        connection.invoke("UserModifiedPlaylist", {
                            partyCode: partyCode,
                            oldTrackIndex: evt.oldIndex,
                            newTrackIndex: evt.newIndex
                        });
                    }
                },
                onAdd: function (evt) {
                    console.log(evt);
                    console.log({
                        partyCode: partyCode,
                        indexToInsertSongAt: evt.newIndex,
                        trackUri: evt.item.dataset.trackuri,
                        albumArtUrl: evt.item.dataset.albumarturl,
                        title: evt.item.dataset.title,
                        artist: evt.item.dataset.artist,
                        length: evt.item.dataset.length
                    });
                    connection.invoke("UserAddedSong", {
                        partyCode: partyCode,
                        indexToInsertSongAt: evt.newIndex,
                        trackUri: evt.item.dataset.trackuri,
                        albumImageUrl: evt.item.dataset.albumimageurl,
                        title: evt.item.dataset.title,
                        artist: evt.item.dataset.artist,
                        length: parseInt(evt.item.dataset.length)
                    });
                }
            });
        }
        function MakeFirstQueueElementHidden() {
            var element = u("#queue").children().first();
            u(element).addClass("hidden");
        }
        function UpdateSong(song) {
            UpdateCurrentSong(song, 0);
            RemoveFirstSongFromQueue();
        }
        function RemoveFirstSongFromQueue() {
            var songToMoveToHistory = u("#queue").children().first();
            u("#history").append(songToMoveToHistory);
            var songInHistory = u("#history").children().last();
            u(songInHistory).removeClass("hidden");
            songToMoveToHistory.remove();
            MakeFirstQueueElementHidden();
        }
    },
    CheckForActiveSpotifyConnection: function CheckForActiveSpotifyConnection() {
        console.log("Checking for active spotify connection");
        fetch("/api/user/CheckSpotifyForConnection")
            .then(function (response) {
            return response.json();
        }).then(function (device) {
            console.log(device);
            if (device.deviceName != null) {
                u(".modal").removeClass("is-active");
                // The user has a device that is connected, we need to update him with where we are in the song
                fetch("/party/UpdateSongForUser")
                    .then(function (response) {
                    if (response.status != 200) {
                        console.log("There was an issue with syncing the music for the user");
                    }
                });
            }
        });
    },
    SyncMusicForUser: function SyncMusicForUser() {
        fetch("/party/UpdateSongForUser")
            .then(function (response) {
            if (response.status != 200) {
                console.log("There was an issue with syncing the music for the user");
            }
        });
    },
    CloseModal: function CloseModal() {
        u(".modal").removeClass("is-active");
    }
};
var Song = /** @class */ (function () {
    function Song() {
    }
    return Song;
}());
var SongModel = /** @class */ (function () {
    function SongModel() {
    }
    return SongModel;
}());
var CurrentSong = /** @class */ (function () {
    function CurrentSong() {
    }
    return CurrentSong;
}());
//# sourceMappingURL=index.js.map