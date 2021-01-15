export class Song {
    name: string;
    artist: string;
    length: number;
    albumImageUrl: string;
    addedBy: string;
}

export class SongModel {
    title: string;
    artist: string;
    length: number;
    albumImageUrl: string;
    trackUri: string;
}

export class TrackSearchModel {
    name: string;
    artist: string;
    album
    length: number;
    uri: string;
    explicit: boolean;
}

export class CurrentSong {
    song?: Song;
    position?: number
}