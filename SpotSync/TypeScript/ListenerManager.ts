const u = require("umbrellajs");

export class ListenerManager {

    private connection: any;

    constructor(signalRConnection: any) {
        this.connection = signalRConnection;

        this.SetUpSignalRListeners()
    }

    private SetUpSignalRListeners() {

        this.connection.on("NewListener", (userName: string) => {
            //console.log(userName)
            u("#listeners").append(`<p>${userName}</p>`)
        })
    }

}