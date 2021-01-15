"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var u = require("umbrellajs");
// Playlist Manager
var PlaylistManager = /** @class */ (function () {
    function PlaylistManager(signalRConnection, partyCode) {
        this.connection = signalRConnection;
        this.partyCode = partyCode;
        this.SetUpUiEvents();
        this.SetUpRealtimeConnections();
    }
    PlaylistManager.prototype.SetUpUiEvents = function () {
        var _this = this;
        u("#generate-playlist-button").on("click", function () {
            _this.SwapGeneratePlaylistButtonWithSyncMusic();
            fetch("/party/UpdateQueueForParty", {
                method: "POST",
                headers: {
                    'Content-Type': 'application/json'
                    // 'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: JSON.stringify({ PartyCode: _this.partyCode })
            });
        });
        u("#toggle-playback").on('click', function (event) {
            // toggle icon
            u("#toggle-playback").toggleClass("fa-pause-circle fa-play-circle");
            fetch("/party/toggleplaybackstate?PartyCode=" + _this.partyCode);
        });
    };
    PlaylistManager.prototype.SetUpRealtimeConnections = function () {
        var _this = this;
        this.connection.on("UpdatePartyView", function (current, history, queue) {
            //console.log("updating the party view")
            if (current.song == null) {
                _this.UpdateHistory(history);
                _this.UpdateQueue(queue);
            }
            else {
                _this.SwapGeneratePlaylistButtonWithSyncMusic();
                _this.UpdateHistory(history);
                _this.UpdateQueue(queue);
                _this.UpdateCurrentSong(current.song, current.position);
            }
        });
        this.connection.on("UpdateSong", function (song) {
            // This is very easy to break
            if (_this.NowPlayingNeedsToUpdate(song)) {
                _this.UpdateSong(song.song);
            }
            else {
                //console.log("Not updating now playing since song is the same")
            }
        });
    };
    PlaylistManager.prototype.SwapGeneratePlaylistButtonWithSyncMusic = function () {
        u("#generate-playlist-button").addClass("hidden");
        u("#sync-music-button").removeClass("hidden");
    };
    PlaylistManager.prototype.UpdateSong = function (song) {
        this.UpdateCurrentSong(song, 0);
        this.MoveCurrentSongFromQueueToHistory();
    };
    PlaylistManager.prototype.NowPlayingNeedsToUpdate = function (track) {
        if (u("#track").text().toLowerCase() == track.song.name.toLowerCase() &&
            u("#artist").text().toLowerCase() == track.song.artist.toLowerCase()) {
            return false;
        }
        else {
            return true;
        }
    };
    PlaylistManager.prototype.UpdateCurrentSong = function (song, position) {
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
    };
    PlaylistManager.prototype.UpdateHistory = function (history) {
        u("#history-loader").removeClass("is-active");
        u("#history").empty();
        if (history.length == 0) {
            this.ShowNoHistory();
        }
        else {
            this.HideNoHistoryDescription();
            history.map(function (song) {
                u("#history").append("<tr><td>" + song.name + "</td><td>" + song.artist + "</td></tr>");
            });
        }
    };
    PlaylistManager.prototype.ShowNoHistory = function () {
        u("#no-history").removeClass("hidden");
    };
    PlaylistManager.prototype.HideNoHistoryDescription = function () {
        u("#no-history").addClass("hidden");
    };
    PlaylistManager.prototype.ShowNoQueue = function () {
        u("#no-queue").removeClass("hidden");
    };
    PlaylistManager.prototype.HideNoQueueDescription = function () {
        u("#no-queue").addClass("hidden");
    };
    PlaylistManager.prototype.UpdateQueue = function (queue) {
        var _this = this;
        u("#queue-loader").removeClass("is-active");
        u("#queue").empty();
        if (queue.length < 2) {
            this.ShowNoQueue();
        }
        else {
            this.HideNoQueueDescription();
            queue.map(function (song) {
                u("#queue").append("<tr><td>" + song.name + "</td><td>" + song.artist + "</td><td>" + (song.addedBy != undefined ? song.addedBy : "") + "</td></tr>");
                //u("#queue").append(`<div>${song.title}</div>`)
            });
        }
        this.MakeFirstQueueElementHidden();
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
                    _this.connection.invoke("UserModifiedPlaylist", {
                        partyCode: _this.partyCode,
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
                _this.connection.invoke("UserAddedSong", {
                    partyCode: _this.partyCode,
                    indexToInsertSongAt: evt.newIndex,
                    trackUri: evt.item.dataset.trackuri,
                    albumImageUrl: evt.item.dataset.albumimageurl,
                    title: evt.item.dataset.title,
                    artist: evt.item.dataset.artist,
                    length: parseInt(evt.item.dataset.length)
                });
            }
        });
    };
    PlaylistManager.prototype.MakeFirstQueueElementHidden = function () {
        var element = u("#queue").children().first();
        u(element).addClass("hidden");
    };
    PlaylistManager.prototype.MoveCurrentSongFromQueueToHistory = function () {
        var songToMoveToHistory = u("#queue").children().first();
        u("#history").append(songToMoveToHistory);
        this.HideNoHistoryDescription();
        var songInHistory = u("#history").children().last();
        u(songInHistory).removeClass("hidden");
        songToMoveToHistory.remove();
        this.MakeFirstQueueElementHidden();
    };
    return PlaylistManager;
}());
exports.PlaylistManager = PlaylistManager;
//# sourceMappingURL=PlaylistManager.js.map