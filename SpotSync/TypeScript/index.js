"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var signalR = require("@microsoft/signalr");
var u = require("umbrellajs");
var rxjs_1 = require("rxjs");
var operators_1 = require("rxjs/operators");
var ajax_1 = require("rxjs/ajax");
module.exports = {
    RealtimeFunctionality: function ConnectToParty(partyCode) {
        document.addEventListener('DOMContentLoaded', function () {
            // MAIN
            //console.log("We loaded the dom content");
        });
        //console.log("Connecting to real time server");
        var connection = new signalR.HubConnectionBuilder().withUrl("/partyhub").build();
        connection.start().then(function () { return connection.invoke("ConnectToParty", partyCode); });
        connection.on("UpdateParty", function (msg) {
            //console.log(msg); 
        });
        connection.on("ExplicitSong", function (message) {
            CreateAndShowModal("Explicit Content", "<p>" + message + "</p>", "<a onclick=\"Spotibro.CloseModal()\" class=\"card-footer-item\">Close</a>");
        });
        u("#generate-playlist-button").on("click", function () {
            console.log("Generating a playlist");
            fetch("/party/UpdateQueueForParty", {
                method: "POST",
                headers: {
                    'Content-Type': 'application/json'
                    // 'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: JSON.stringify({ PartyCode: partyCode })
            });
        });
        connection.on("UpdatePartyView", function (current, history, queue) {
            //console.log("updating the party view")
            HideNowPlayingLoader();
            if (current.song == null) {
                //console.log("current is undefined or null")
                //console.log(current);
                ShowNoCurrentPlayingSong();
                UpdateHistory(history);
                UpdateQueue(queue);
            }
            else {
                //console.log("current is not undefined or null")
                //console.log(current);
                ShowNowPlayingSong();
                UpdateHistory(history);
                UpdateQueue(queue);
                UpdateCurrentSong(current.song, current.position);
            }
            /*
            console.log("current:")
            console.log(current);
            console.log("history:")
            console.log(history);
            console.log("queue");
            console.log(queue);
            */
        });
        function HideNowPlayingLoader() {
            u("#now-playing-loader").removeClass("is-active");
        }
        function ShowNowPlayingSong() {
            u("#now-playing-container").removeClass("hidden");
        }
        function ShowNoCurrentPlayingSong() {
            u("#no-songs-playing-container").removeClass("hidden");
        }
        connection.on("UpdateSong", function (song) {
            // This is very easy to break
            if (NowPlayingNeedsToUpdate(song)) {
                HideNoNowPlayingSongDescription();
                UpdateSong(song.song);
            }
            else {
                //console.log("Not updating now playing since song is the same")
            }
        });
        function NowPlayingNeedsToUpdate(track) {
            if (u("#track").text().toLowerCase() == track.song.name.toLowerCase() &&
                u("#artist").text().toLowerCase() == track.song.artist.toLowerCase()) {
                return false;
            }
            else {
                return true;
            }
        }
        connection.on("NewListener", function (userName) {
            //console.log(userName)
            u("#listeners").append("<p>" + userName + "</p>");
        });
        connection.on("ConnectSpotify", function (msg) {
            //console.log("Users Spotify is disconnected");
            u("#card-content").html();
            u(".modal").addClass("is-active");
            CreateAndShowModal("Where is your Spotify?", "<p> It looks like your Spotify got disconnected from us.This might be because your Spotify app was closed.Make sure your Spotify app is open on whatever device you want to listen through and start playing a song and press the button below to see if we can find your Spotify </p>", "<a onclick=\"Spotibro.CheckForActiveSpotifyConnection()\" class=\"card-footer-item\">Sync</a>", "<a onclick=\"Spotibro.CloseModal()\" class=\"card-footer-item\">Close</a>");
        });
        //<a onclick="Spotibro.CheckForActiveSpotifyConnection()" class="card-footer-item">Sync</a>
        //<a onclick="Spotibro.CloseModal()" class="card-footer-item" > Close < /a>
        function CreateAndShowModal(htmlTitle, htmlMessage, button1Html, button2Html) {
            if (button2Html === void 0) { button2Html = undefined; }
            u("#card-header").html(htmlTitle);
            u("#card-content").html(htmlMessage);
            u("#card-footer").empty();
            u("#card-footer").append(button1Html);
            if (button2Html != undefined) {
                u("#card-footer").append(button2Html);
            }
            u(".modal").addClass("is-active");
        }
        /*
        fromEvent(document.getElementById("search-spotify-input"), 'input').subscribe(event => {
            u("#results").empty();
            u("#results").siblings("#search-loader").addClass("is-active");
        })

        u("#is-search-spotify-wrapper").on("mouseleave", (event) => {
            u("#is-search-spotify-wrapper").removeClass("increment-search-box")
            u("#results").addClass("hidden")
        })

        u("#search-spotify-input").on("input", (event) => {
            u("#is-search-spotify-wrapper").addClass("increment-search-box")
            u("#results").removeClass("hidden")
        })
        */
        var notificationManager = new NotificationManager();
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
        // This closes the search container
        document.addEventListener("click", function (event) {
            if (u(event.target).closest("#search-outline").length == 0) {
                u("#search-results-container").addClass("hidden");
            }
        });
        rxjs_1.fromEvent(document.getElementById("search-spotify-input"), 'click').pipe(operators_1.tap(function (event) {
            u("#suggested-tracks").empty();
            u("#search-results-container").removeClass("hidden");
            u("#search-results").addClass("hidden");
            u("#suggested-search-loader").addClass("is-active");
            u(".track-view").empty();
        })).subscribe(function (event) {
            //console.log("Someone clicked the input container")
            //console.log(event);
            var suggestedSongCache = new SearchCache();
            ajax_1.ajax.getJSON('/api/user/suggestedsongs?limit=10').pipe(operators_1.catchError(function (error) {
                return LogError(error);
            })).subscribe(function (tracks) {
                u("#suggested-search-loader").removeClass("is-active");
                AddSuggestedSongsToView(tracks, suggestedSongCache);
                AddEventListenersToSuggestedTracks(suggestedSongCache);
            });
        });
        function AddSuggestedSongsToView(suggestedSongs, cache) {
            suggestedSongs.map(function (song) {
                cache.SuggestedTracks[song.uri] = song;
                u("#suggested-tracks").append(CreateTrackViewHtml(song));
            });
        }
        rxjs_1.fromEvent(document.getElementById("search-spotify-input"), 'input').pipe(operators_1.filter(function (event) { return event.target.value.length > 0; }), operators_1.tap(function (event) {
            u("#search-results-container").removeClass("hidden");
            u("#search-results").addClass("hidden");
            u("#track-search-loader").addClass("is-active");
            u(".track-view").empty();
        }), operators_1.debounce(function () { return rxjs_1.interval(1500); }))
            .subscribe(function (event) {
            //console.log((<HTMLInputElement>event.target).value);
            var cache = new SearchCache();
            ajax_1.ajax.getJSON("/api/user/searchSpotify?query=" + event.target.value + "&queryType=0").pipe(operators_1.catchError(function (error) {
                return LogError(error);
            })).subscribe(function (response) {
                RemoveTrackLoaderAndShowSearchResults();
                AddTracksToView(response, cache);
                AddEventListenersToSearchedTracks(cache);
            });
        });
        function LogError(error) {
            //console.log('error: ', error);
            return rxjs_1.of(error);
        }
        function AddTracksToView(tracks, cache) {
            tracks.map(function (song) {
                cache.Tracks[song.uri] = song;
                u(".track-view").append(CreateTrackViewHtml(song));
            });
        }
        function RemoveTrackLoaderAndShowSearchResults() {
            u("#track-search-loader").removeClass("is-active");
            u("#search-results").removeClass("hidden");
        }
        function AddEventListenersToSearchedTracks(cache) {
            u("#add").on('click', function (event) {
                var song = cache.Tracks[event.target.dataset.uri];
                connection.invoke("UserAddedSong", {
                    partyCode: partyCode,
                    indexToInsertSongAt: -1,
                    trackUri: song.uri,
                    albumImageUrl: null,
                    name: song.name,
                    artist: song.artist,
                    length: song.length,
                    explicit: song.explicit
                });
                NotifyUserSongAdded(song);
            });
        }
        function AddEventListenersToSuggestedTracks(cache) {
            u("#add").on('click', function (event) {
                var song = cache.SuggestedTracks[event.target.dataset.uri];
                connection.invoke("UserAddedSong", {
                    partyCode: partyCode,
                    indexToInsertSongAt: -1,
                    trackUri: song.uri,
                    albumImageUrl: null,
                    name: song.name,
                    artist: song.artist,
                    length: song.length
                });
                NotifyUserSongAdded(song);
            });
        }
        function NotifyUserSongAdded(song) {
            notificationManager.ShowMessage("Successfully added " + song.name + " to the queue.", 5000);
        }
        function CreateTrackViewHtml(song) {
            //console.log(song);
            return "<div class=\"track-item\">\n                        <div>\n                            <p class=\"track-title\">" + song.name + "</p>\n                            <p class=\"artist\">" + song.artist + "</p>\n                        </div>\n                        <div class=\"add-icon\">\n                            <i data-uri=\"" + song.uri + "\" id=\"add\" class=\"fas fa-plus\"></i>\n                        </div>\n                    </div>\n                    ";
        }
        function AddTrackToEndOfPlaylist(uri, name, artist, length) {
            connection.invoke("UserAddedSong", {
                partyCode: partyCode,
                indexToInsertSongAt: -1,
                trackUri: uri,
                albumImageUrl: null,
                title: name,
                artist: artist,
                length: length
            });
        }
        ;
        function UpdateCurrentSong(song, position) {
            if (song.albumImageUrl == undefined || song.albumImageUrl == null) {
                document.getElementById("albumArt").setAttribute("src", "./assets/unknown-album-art.png");
                //console.log("there is not a valid image url");
            }
            else {
                //console.log("there is a valid albumImageUrl: " + song.albumImageUrl)
                document.getElementById("albumArt").setAttribute("src", song.albumImageUrl);
            }
            document.getElementById("track").innerText = song.name;
            document.getElementById("artist").innerText = song.artist;
        }
        function UpdateHistory(history) {
            u("#history-loader").removeClass("is-active");
            u("#history").empty();
            if (history.length == 0) {
                ShowNoHistory();
            }
            else {
                HideNoHistoryDescription();
                history.map(function (song) {
                    u("#history").append("<tr><td>" + song.name + "</td><td>" + song.artist + "</td></tr>");
                });
            }
        }
        function ShowNoHistory() {
            u("#no-history").removeClass("hidden");
        }
        function HideNoHistoryDescription() {
            u("#no-history").addClass("hidden");
        }
        function ShowNoQueue() {
            u("#no-queue").removeClass("hidden");
        }
        function HideNoQueueDescription() {
            u("#no-queue").addClass("hidden");
        }
        function HideNoNowPlayingSongDescription() {
            u("#no-songs-playing-container").addClass("hidden");
        }
        function UpdateQueue(queue) {
            u("#queue-loader").removeClass("is-active");
            u("#queue").empty();
            if (queue.length < 2) {
                ShowNoQueue();
            }
            else {
                HideNoQueueDescription();
                queue.map(function (song) {
                    u("#queue").append("<tr><td>" + song.name + "</td><td>" + song.artist + "</td><td>" + (song.addedBy != undefined ? song.addedBy : "") + "</td></tr>");
                    //u("#queue").append(`<div>${song.title}</div>`)
                });
            }
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
                    //console.log(evt);
                    if (evt.oldIndex !== evt.newIndex) {
                        connection.invoke("UserModifiedPlaylist", {
                            partyCode: partyCode,
                            oldTrackIndex: evt.oldIndex,
                            newTrackIndex: evt.newIndex
                        });
                    }
                },
                onAdd: function (evt) {
                    //console.log(evt);
                    /*console.log({
                        partyCode: partyCode,
                        indexToInsertSongAt: evt.newIndex,
                        trackUri: evt.item.dataset.trackuri,
                        albumArtUrl: evt.item.dataset.albumarturl,
                        title: evt.item.dataset.title,
                        artist: evt.item.dataset.artist,
                        length: evt.item.dataset.length
                    })*/
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
            MoveCurrentSongFromQueueToHistory();
        }
        function MoveCurrentSongFromQueueToHistory() {
            var songToMoveToHistory = u("#queue").children().first();
            u("#history").append(songToMoveToHistory);
            HideNoHistoryDescription();
            var songInHistory = u("#history").children().last();
            u(songInHistory).removeClass("hidden");
            songToMoveToHistory.remove();
            MakeFirstQueueElementHidden();
        }
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
var NotificationManager = /** @class */ (function () {
    function NotificationManager() {
    }
    NotificationManager.prototype.ShowMessage = function (message, duration) {
        if (duration === void 0) { duration = 5000; }
        var id = stringGen(5);
        u("#user-notifications").append(this.GetNotification(message, id));
        setTimeout(function () {
            u("#" + id).remove();
        }, duration);
    };
    NotificationManager.prototype.GetNotification = function (message, id) {
        return "<div class=\"notification is-info\" id=\"" + id + "\">\n                    " + message + "\n                </div>";
    };
    return NotificationManager;
}());
function stringGen(len) {
    var text = "";
    var charset = "abcdefghijklmnopqrstuvwxyz";
    for (var i = 0; i < len; i++)
        text += charset.charAt(Math.floor(Math.random() * charset.length));
    return text;
}
var SearchCache = /** @class */ (function () {
    function SearchCache() {
        this.Tracks = {};
        this.SuggestedTracks = {};
    }
    return SearchCache;
}());
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
var TrackSearchModel = /** @class */ (function () {
    function TrackSearchModel() {
    }
    return TrackSearchModel;
}());
var CurrentSong = /** @class */ (function () {
    function CurrentSong() {
    }
    return CurrentSong;
}());
//# sourceMappingURL=index.js.map