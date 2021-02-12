"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var u = require("umbrellajs");
var rxjs_1 = require("rxjs");
var operators_1 = require("rxjs/operators");
var ajax_1 = require("rxjs/ajax");
var SearchCache_1 = require("../TypeScript/SearchCache");
var NotificationManager_1 = require("../TypeScript/NotificationManager");
var SearchContainerManager = /** @class */ (function () {
    function SearchContainerManager(signalRConnection, partyCode) {
        console.log("In constructor for SearchContainerManager");
        this.connection = signalRConnection;
        this.partyCode = partyCode;
        this.notificationManager = new NotificationManager_1.NotificationManager();
        this.SetUpOnClickListeners();
        this.SetUpSignalRListeners();
    }
    SearchContainerManager.prototype.SetUpOnClickListeners = function () {
        var _this = this;
        document.addEventListener("click", function (event) {
            // This closes the search container
            if (u(event.target).closest("#search-outline").length == 0) {
                u("#search-results-container").addClass("hidden");
            }
        });
        // When someone clicks on the search input
        rxjs_1.fromEvent(document.getElementById("search-spotify-input"), 'click').pipe(operators_1.tap(function (event) {
            u("#suggested-tracks").empty();
            u("#search-results-container").removeClass("hidden");
            u("#search-results").addClass("hidden");
            u("#suggested-search-loader").addClass("is-active");
            u(".track-view").empty();
        })).subscribe(function (event) {
            var suggestedSongCache = new SearchCache_1.SearchCache();
            ajax_1.ajax.getJSON('/api/user/suggestedsongs?limit=10').pipe(operators_1.catchError(function (error) {
                return _this.LogError(error);
            })).subscribe(function (tracks) {
                u("#suggested-search-loader").removeClass("is-active");
                _this.AddSuggestedSongsToView(tracks, suggestedSongCache);
                _this.AddEventListenersToSuggestedTracks(suggestedSongCache);
            });
        });
        // When someone is typing in the input
        rxjs_1.fromEvent(document.getElementById("search-spotify-input"), 'input').pipe(operators_1.filter(function (event) { return event.target.value.length > 0; }), operators_1.tap(function (event) {
            u("#search-results-container").removeClass("hidden");
            u("#search-results").addClass("hidden");
            u("#track-search-loader").addClass("is-active");
            u(".track-view").empty();
        }), operators_1.debounce(function () { return rxjs_1.interval(1500); }))
            .subscribe(function (event) {
            //console.log((<HTMLInputElement>event.target).value);
            var cache = new SearchCache_1.SearchCache();
            ajax_1.ajax.getJSON("/api/user/searchSpotify?query=" + event.target.value + "&queryType=0").pipe(operators_1.catchError(function (error) {
                return _this.LogError(error);
            })).subscribe(function (response) {
                _this.RemoveTrackLoaderAndShowSearchResults();
                _this.AddTracksToView(response, cache);
                _this.AddEventListenersToSearchedTracks(cache);
            });
        });
    };
    SearchContainerManager.prototype.LogError = function (error) {
        //console.log('error: ', error);
        return rxjs_1.of(error);
    };
    SearchContainerManager.prototype.SetUpSignalRListeners = function () {
    };
    SearchContainerManager.prototype.AddSuggestedSongsToView = function (suggestedSongs, cache) {
        var _this = this;
        suggestedSongs.map(function (song) {
            cache.SuggestedTracks[song.uri] = song;
            u("#suggested-tracks").append(_this.CreateTrackViewHtml(song));
        });
    };
    SearchContainerManager.prototype.AddTracksToView = function (tracks, cache) {
        var _this = this;
        tracks.map(function (song) {
            cache.Tracks[song.uri] = song;
            u(".track-view").append(_this.CreateTrackViewHtml(song));
        });
    };
    SearchContainerManager.prototype.RemoveTrackLoaderAndShowSearchResults = function () {
        u("#track-search-loader").removeClass("is-active");
        u("#search-results").removeClass("hidden");
    };
    SearchContainerManager.prototype.AddEventListenersToSearchedTracks = function (cache) {
        var _this = this;
        u("#add").on('click', function (event) {
            var song = cache.Tracks[event.target.dataset.uri];
            _this.connection.invoke("UserAddedSong", {
                partyCode: _this.partyCode,
                indexToInsertSongAt: -1,
                trackUri: song.uri,
                albumImageUrl: null,
                name: song.name,
                artist: song.artist,
                length: song.length,
                explicit: song.explicit
            });
            _this.NotifyUserSongAdded(song);
        });
    };
    SearchContainerManager.prototype.AddEventListenersToSuggestedTracks = function (cache) {
        var _this = this;
        u("#add").on('click', function (event) {
            var song = cache.SuggestedTracks[event.target.dataset.uri];
            _this.connection.invoke("UserAddedSong", {
                partyCode: _this.partyCode,
                indexToInsertSongAt: -1,
                trackUri: song.uri,
                albumImageUrl: null,
                name: song.name,
                artist: song.artist,
                length: song.length
            });
            _this.NotifyUserSongAdded(song);
        });
    };
    SearchContainerManager.prototype.NotifyUserSongAdded = function (song) {
        this.notificationManager.ShowMessage("Successfully added " + song.name + " to the queue.", 3000);
    };
    SearchContainerManager.prototype.CreateTrackViewHtml = function (song) {
        //console.log(song);
        return "<div class=\"track-item\">\n                        <div>\n                            <p class=\"track-title\">" + song.name + "</p>\n                            <p class=\"artist\">" + song.artist + "</p>\n                        </div>\n                        <div class=\"add-icon\">\n                            <i data-uri=\"" + song.uri + "\" id=\"add\" class=\"fas fa-plus\"></i>\n                        </div>\n                    </div>\n                    ";
    };
    return SearchContainerManager;
}());
exports.SearchContainerManager = SearchContainerManager;
//# sourceMappingURL=SearchContainerManager.js.map