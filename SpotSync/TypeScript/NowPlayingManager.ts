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

        document.addEventListener("click", (event: any) => {
            console.log(event);
            // This closes the search container
            if (u(event.target).closest("#more-information-popup").length == 0 && event.target.id != "toggle-information-menu") {
                u("#more-information-popup").addClass("hidden");
            }
        })

        document.addEventListener("click", (event: any) => {
            console.log(event);
            // This closes the search container
            if (u(event.target).closest("#devices-popup").length == 0 && event.target.id != "toggle-devices-popup") {
                u("#devices-popup").addClass("hidden");
            }
        })

        u("#toggle-devices-popup").on("click", event => {
            u("#devices-popup").toggleClass("hidden");
            this.devicePopupClearAndShowLoading()
            // TODO: Fix this. If the user clicks the icon to hide it, this will cause a request be sent to the server
            this.getUsersAvailableDevices();
        })

        u("#refresh-devices").on("click", event => {
            this.devicePopupClearAndShowLoading()
            // TODO: Fix this. If the user clicks the icon to hide it, this will cause a request be sent to the server
            this.getUsersAvailableDevices();
        })
    }

    private devicePopupClearAndShowLoading() {
        u("#active-device").empty();
        u("#device-loader").addClass("is-active");
    }

    private getUsersAvailableDevices() {
        fetch("/api/user/getactivedevices")
            .then(response => response.json())
            .then(devices => {
                if (devices.type != undefined && devices.type == "error") {
                    console.log("There was an error reaching Spotifies API");
                } else {
                    devices.map(device => {
                        this.hideLoader();
                        this.displayDeviceInPopUp(device);
                    })
                }

            });
    }

    private hideLoader() {

        u("#device-loader").removeClass("is-active");
    }

    private displayDeviceInPopUp(playbackDevice: Device) {
        u("#active-device").append(this.createHtmlForDevice(playbackDevice));
    }

    private createHtmlForDevice(playbackDevice: Device): string {
        return `<p class="spotibro-text device ${this.IsActiveCss(playbackDevice)}">${playbackDevice.name}</p>`
    }

    private IsActiveCss(playbackDevice: Device): string {
        if (playbackDevice.active) {
            return 'active-device';
        } else {
            return '';
        }
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

class Device {
    name: string;
    id: string;
    active: boolean;
}