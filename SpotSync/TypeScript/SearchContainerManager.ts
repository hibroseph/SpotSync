const u = require("umbrellajs");
import { fromEvent, interval, of } from 'rxjs';
import { debounce, map, catchError, startWith, tap, filter } from 'rxjs/operators';
import { ajax } from 'rxjs/ajax';
import { SearchCache } from '../TypeScript/SearchCache';
import { TrackSearchModel } from "../TypeScript/Types";
import { NotificationManager } from "../TypeScript/NotificationManager";

export class SearchContainerManager {
    private partyCode: string;
    private connection: any;
    private notificationManager: NotificationManager;

    constructor(signalRConnection: any, partyCode: string) {
        console.log("In constructor for SearchContainerManager")
        this.connection = signalRConnection;
        this.partyCode = partyCode;
        this.notificationManager = new NotificationManager();

        this.SetUpOnClickListeners();
        this.SetUpSignalRListeners();
    }

    private SetUpOnClickListeners() {
        document.addEventListener("click", (event) => {
            // This closes the search container
            if (u(event.target).closest("#search-outline").length == 0) {
                u("#search-results-container").addClass("hidden");
            }
        })

        // When someone clicks on the search input
        fromEvent(document.getElementById("search-spotify-input"), 'click').pipe(
            tap((event) => {
                u("#suggested-tracks").empty();
                u("#search-results-container").removeClass("hidden");
                u("#search-results").addClass("hidden");
                u("#suggested-search-loader").addClass("is-active");
                u(".track-view").empty();
            })
        ).subscribe(event => {

            let suggestedSongCache: SearchCache = new SearchCache();

            ajax.getJSON('/api/user/suggestedsongs?limit=10').pipe(
                catchError(error => {
                    return this.LogError(error);
                })).subscribe(tracks => {
                    u("#suggested-search-loader").removeClass("is-active")
                    this.AddSuggestedSongsToView(tracks, suggestedSongCache);
                    this.AddEventListenersToSuggestedTracks(suggestedSongCache);
                })
        });

        // When someone is typing in the input
        fromEvent(document.getElementById("search-spotify-input"), 'input').pipe(
            filter((event) => (<HTMLInputElement>event.target).value.length > 0),
            tap((event) => {
                u("#search-results-container").removeClass("hidden");
                u("#search-results").addClass("hidden");
                u("#track-search-loader").addClass("is-active");
                u(".track-view").empty();
            }),
            debounce(() => interval(1500)))
            .subscribe(event => {
                //console.log((<HTMLInputElement>event.target).value);

                let cache: SearchCache = new SearchCache();

                ajax.getJSON(`/api/user/searchSpotify?query=${(<HTMLInputElement>event.target).value}&queryType=0`).pipe(
                    catchError(error => {
                        return this.LogError(error);
                    })).subscribe(response => {
                        this.RemoveTrackLoaderAndShowSearchResults();

                        this.AddTracksToView(response, cache);

                        this.AddEventListenersToSearchedTracks(cache);
                    })
            });

    }

    private LogError(error) {
        //console.log('error: ', error);
        return of(error);
    }

    private SetUpSignalRListeners() {

    }

    private AddSuggestedSongsToView(suggestedSongs: TrackSearchModel[], cache: SearchCache) {
        suggestedSongs.map(song => {
            cache.SuggestedTracks[song.uri] = song
            u("#suggested-tracks").append(this.CreateTrackViewHtml(song))
        })
    }

    private AddTracksToView(tracks: TrackSearchModel[], cache: SearchCache) {
        tracks.map(song => {
            cache.Tracks[song.uri] = song;
            u(".track-view").append(this.CreateTrackViewHtml(song))
        })
    }

    private RemoveTrackLoaderAndShowSearchResults() {
        u("#track-search-loader").removeClass("is-active");
        u("#search-results").removeClass("hidden");
    }

    private AddEventListenersToSearchedTracks(cache: SearchCache) {
        u("#add").on('click', (event) => {
            let song = cache.Tracks[event.target.dataset.uri]
            this.connection.invoke("UserAddedSong", {
                partyCode: this.partyCode,
                indexToInsertSongAt: -1,
                trackUri: song.uri,
                albumImageUrl: null,
                name: song.name,
                artist: song.artist,
                length: song.length,
                explicit: song.explicit
            })

            this.NotifyUserSongAdded(song);
        })
    }

    private AddEventListenersToSuggestedTracks(cache: SearchCache) {
        u("#add").on('click', (event) => {
            let song = cache.SuggestedTracks[event.target.dataset.uri]
            this.connection.invoke("UserAddedSong", {
                partyCode: this.partyCode,
                indexToInsertSongAt: -1,
                trackUri: song.uri,
                albumImageUrl: null,
                name: song.name,
                artist: song.artist,
                length: song.length
            })

            this.NotifyUserSongAdded(song);
        })
    }

    private NotifyUserSongAdded(song: TrackSearchModel) {
        this.notificationManager.ShowMessage(`Successfully added ${song.name} to the queue.`, 50000);
    }

    private CreateTrackViewHtml(song: TrackSearchModel) {
        //console.log(song);
        return `<div class="track-item">
                        <div>
                            <p class="track-title">${song.name}</p>
                            <p class="artist">${song.artist}</p>
                        </div>
                        <div class="add-icon">
                            <i data-uri="${song.uri}" id="add" class="fas fa-plus"></i>
                        </div>
                    </div>
                    `
    }
}