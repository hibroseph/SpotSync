const u = require("umbrellajs");
import { CurrentSong, Song } from '../TypeScript/Types';

// Playlist Manager
export class PlaylistManager {

    private connection: any;
    private partyCode: string;

    constructor(signalRConnection: any, partyCode: string) {
        this.connection = signalRConnection;
        this.partyCode = partyCode;
        this.SetUpUiEvents()
        this.SetUpRealtimeConnections()
    }

    private SetUpUiEvents() {
        u("#generate-playlist-button").on("click", () => {
            this.SwapGeneratePlaylistButtonWithSyncMusic();
            fetch("/party/UpdateQueueForParty", {
                method: "POST",
                headers: {
                    'Content-Type': 'application/json'
                    // 'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: JSON.stringify({ PartyCode: this.partyCode })
            })
        });

        u("#toggle-playback").on('click', (event) => {
            // toggle icon
            u("#toggle-playback").toggleClass("fa-pause-circle fa-play-circle")
            fetch(`/party/toggleplaybackstate?PartyCode=${this.partyCode}`);
        })


    }

    private SetUpRealtimeConnections() {
        this.connection.on("UpdatePartyView", (current: CurrentSong, history: Song[], queue: Song[]) => {
            //console.log("updating the party view")

            if (current.song == null) {
                this.UpdateHistory(history);
                this.UpdateQueue(queue);
            } else {
                this.SwapGeneratePlaylistButtonWithSyncMusic();
                this.UpdateHistory(history);
                this.UpdateQueue(queue);
                this.UpdateCurrentSong(current.song, current.position);
            }
        })

        this.connection.on("UpdateSong", (song: CurrentSong) => {
            // This is very easy to break
            if (this.NowPlayingNeedsToUpdate(song)) {
                this.UpdateSong(song.song);
            } else {
                //console.log("Not updating now playing since song is the same")
            }
        })
    }

    private SwapGeneratePlaylistButtonWithSyncMusic() {
        u("#generate-playlist-button").addClass("hidden");
        u("#sync-music-button").removeClass("hidden");
    }

    private UpdateSong(song: Song) {
        this.UpdateCurrentSong(song, 0);
        this.MoveCurrentSongFromQueueToHistory();
    }

    private NowPlayingNeedsToUpdate(track: CurrentSong): boolean {
        if (u("#track").text().toLowerCase() == track.song.name.toLowerCase() &&
            u("#artist").text().toLowerCase() == track.song.artist.toLowerCase()) {
            return false;
        } else {
            return true;
        }
    }

    private UpdateCurrentSong(song: Song, position: number) {
        if (song.albumImageUrl == undefined || song.albumImageUrl == null) {
            document.getElementById("albumArt").setAttribute("src", "./assets/unknown-album-art.png");

            //console.log("there is not a valid image url");
        } else {
            //console.log("there is a valid albumImageUrl: " + song.albumImageUrl)
            document.getElementById("albumArt").setAttribute("src", song.albumImageUrl);
        }
        document.getElementById("track").innerText = song.name;
        document.getElementById("artist").innerText = song.artist;
    }


    private UpdateHistory(history: Song[]) {
        u("#history-loader").removeClass("is-active");
        u("#history").empty();
        if (history.length == 0) {
            this.ShowNoHistory();
        } else {
            this.HideNoHistoryDescription()

            history.map(song => {
                u("#history").append(`<tr><td>${song.name}</td><td>${song.artist}</td></tr>`)
            })
        }
    }

    private ShowNoHistory() {
        u("#no-history").removeClass("hidden");
    }

    private HideNoHistoryDescription() {
        u("#no-history").addClass("hidden")
    }

    private ShowNoQueue() {
        u("#no-queue").removeClass("hidden");
    }

    private HideNoQueueDescription() {
        u("#no-queue").addClass("hidden")
    }

    private UpdateQueue(queue: Song[]) {
        u("#queue-loader").removeClass("is-active");
        u("#queue").empty();

        if (queue.length < 2) {
            this.ShowNoQueue();
        } else {
            this.HideNoQueueDescription();
            queue.map(song => {
                u("#queue").append(`<tr><td>${song.name}</td><td>${song.artist}</td><td>${song.addedBy != undefined ? song.addedBy : ""}</td></tr>`)
                //u("#queue").append(`<div>${song.title}</div>`)
            })
        }

        this.MakeFirstQueueElementHidden()

        let element = document.getElementById("queue");
        // @ts-ignore
        var sortable = Sortable.create(element, {
            group: 'shared',
            animation: 300,
            easing: "cubic-bezier(0.76, 0, 0.24, 1)",
            ghostClass: "has-background-white-ter",
            chosenClass: "has-background-info-light",
            dragClass: "has-background-info-light",
            onEnd: (evt) => {
                //console.log(evt);

                if (evt.oldIndex !== evt.newIndex) {
                    this.connection.invoke("UserModifiedPlaylist", {
                        partyCode: this.partyCode,
                        oldTrackIndex: evt.oldIndex,
                        newTrackIndex: evt.newIndex
                    })
                }
            },
            onAdd: (evt) => {
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

                this.connection.invoke("UserAddedSong", {
                    partyCode: this.partyCode,
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

    private MakeFirstQueueElementHidden() {
        let element = u("#queue").children().first();
        u(element).addClass("hidden");
    }


    private MoveCurrentSongFromQueueToHistory() {
        let songToMoveToHistory = u("#queue").children().first();
        u("#history").append(songToMoveToHistory);
        this.HideNoHistoryDescription();
        let songInHistory = u("#history").children().last();
        u(songInHistory).removeClass("hidden");
        songToMoveToHistory.remove();
        this.MakeFirstQueueElementHidden();
    }
}