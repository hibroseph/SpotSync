const u = require("umbrellajs");

export class NowPlayingManager {
    private skipUiElement: any;
    private togglePlaybackUiElement: any;
    private signalRConnection: any;
    private partyCode: string;

    constructor(signalRConnection, partyCode) {
        this.skipUiElement = u("#skip");
        this.togglePlaybackUiElement = u("toggle-playback");
        this.signalRConnection = signalRConnection;
        this.partyCode = partyCode;
        this.setUpOnClickListeners();
    }

    private setUpOnClickListeners = () => {
        this.skipUiElement.on('click', this.skipUiOnClickCallback)
        this.togglePlaybackUiElement.on('on', this.togglePlaybackCallback)

        u("#toggle-information-menu").on("click", (event) => {
            u("#more-information-popup").toggleClass("hidden");
        })
    }

    private skipUiOnClickCallback = () => {
        console.log(this.signalRConnection);
        this.signalRConnection.invoke("UserWantsToSkipSong", this.partyCode);
    }

    private togglePlaybackCallback = () => {
        this.togglePlaybackUiElement.on('click', (event) => {
            u("#toggle-playback").toggleClass("fa-pause-circle fa-play-circle")
            fetch(`/party/toggleplaybackstate?PartyCode=${this.partyCode}`);
        })
    }


}