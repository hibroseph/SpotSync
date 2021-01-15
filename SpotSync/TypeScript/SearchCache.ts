import { TrackSearchModel } from "../TypeScript/Types";

export class SearchCache {

    constructor() {
        this.Tracks = {}
        this.SuggestedTracks = {}
    }

    Tracks: Record<string, TrackSearchModel>
    SuggestedTracks: Record<string, TrackSearchModel>
}