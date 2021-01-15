const u = require("umbrellajs");

export class ModalManager {
    private connection: any;

    constructor(signalRConnection: any) {
        this.connection = signalRConnection;
    }

    setUpSignalRListeners() {
        this.connection.on("ExplicitSong", (message: string) => {
            this.CreateAndShowModal("Explicit Content", `<p>${message}</p>`, `<a onclick="Spotibro.CloseModal()" class="card-footer-item">Close</a>`);
        })

        this.connection.on("ConnectSpotify", (msg: string) => {
            //console.log("Users Spotify is disconnected");

            u("#card-content").html();
            u(".modal").addClass("is-active")

            this.CreateAndShowModal("Where is your Spotify?", "<p> It looks like your Spotify got disconnected from us.This might be because your Spotify app was closed.Make sure your Spotify app is open on whatever device you want to listen through and start playing a song and press the button below to see if we can find your Spotify </p>", `<a onclick="Spotibro.CheckForActiveSpotifyConnection()" class="card-footer-item">Sync</a>`, `<a onclick="Spotibro.CloseModal()" class="card-footer-item">Close</a>`)
        })
    }

    CreateAndShowModal(htmlTitle, htmlMessage, button1Html, button2Html = undefined) {
        u("#card-header").html(htmlTitle);
        u("#card-content").html(htmlMessage);

        u("#card-footer").empty();
        u("#card-footer").append(button1Html);

        if (button2Html != undefined) {
            u("#card-footer").append(button2Html);
        }

        u(".modal").addClass("is-active")
    }

}