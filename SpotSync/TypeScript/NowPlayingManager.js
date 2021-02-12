"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var u = require("umbrellajs");
var NowPlayingManager = /** @class */ (function () {
    function NowPlayingManager(signalRConnection, partyCode) {
        var _this = this;
        this.setUpOnClickListeners = function () {
            _this.skipUiElement.on('click', _this.skipUiOnClickCallback);
            _this.togglePlaybackUiElement.on('on', _this.togglePlaybackCallback);
            u("#toggle-information-menu").on("click", function (event) {
                u("#more-information-popup").toggleClass("hidden");
            });
            document.addEventListener("click", function (event) {
                console.log(event);
                // This closes the search container
                if (u(event.target).closest("#more-information-popup").length == 0 && event.target.id != "toggle-information-menu") {
                    u("#more-information-popup").addClass("hidden");
                }
            });
            document.addEventListener("click", function (event) {
                console.log(event);
                // This closes the search container
                if (u(event.target).closest("#devices-popup").length == 0 && event.target.id != "toggle-devices-popup") {
                    u("#devices-popup").addClass("hidden");
                }
            });
            u("#toggle-devices-popup").on("click", function (event) {
                u("#devices-popup").toggleClass("hidden");
                _this.devicePopupClearAndShowLoading();
                // TODO: Fix this. If the user clicks the icon to hide it, this will cause a request be sent to the server
                _this.getUsersAvailableDevices();
            });
            u("#refresh-devices").on("click", function (event) {
                _this.devicePopupClearAndShowLoading();
                // TODO: Fix this. If the user clicks the icon to hide it, this will cause a request be sent to the server
                _this.getUsersAvailableDevices();
            });
        };
        this.skipUiOnClickCallback = function () {
            console.log(_this.signalRConnection);
            _this.signalRConnection.invoke("UserWantsToSkipSong", _this.partyCode);
        };
        this.togglePlaybackCallback = function () {
            _this.togglePlaybackUiElement.on('click', function (event) {
                u("#toggle-playback").toggleClass("fa-pause-circle fa-play-circle");
                fetch("/party/toggleplaybackstate?PartyCode=" + _this.partyCode);
            });
        };
        this.skipUiElement = u("#skip");
        this.togglePlaybackUiElement = u("toggle-playback");
        this.signalRConnection = signalRConnection;
        this.partyCode = partyCode;
        this.setUpOnClickListeners();
    }
    NowPlayingManager.prototype.devicePopupClearAndShowLoading = function () {
        u("#active-device").empty();
        u("#device-loader").addClass("is-active");
    };
    NowPlayingManager.prototype.getUsersAvailableDevices = function () {
        var _this = this;
        fetch("/api/user/getactivedevices")
            .then(function (response) { return response.json(); })
            .then(function (devices) {
            if (devices.type != undefined && devices.type == "error") {
                console.log("There was an error reaching Spotifies API");
            }
            else {
                devices.map(function (device) {
                    _this.hideLoader();
                    _this.displayDeviceInPopUp(device);
                });
            }
        });
    };
    NowPlayingManager.prototype.hideLoader = function () {
        u("#device-loader").removeClass("is-active");
    };
    NowPlayingManager.prototype.displayDeviceInPopUp = function (playbackDevice) {
        u("#active-device").append(this.createHtmlForDevice(playbackDevice));
    };
    NowPlayingManager.prototype.createHtmlForDevice = function (playbackDevice) {
        return "<p class=\"spotibro-text device " + this.IsActiveCss(playbackDevice) + "\">" + playbackDevice.name + "</p>";
    };
    NowPlayingManager.prototype.IsActiveCss = function (playbackDevice) {
        if (playbackDevice.active) {
            return 'active-device';
        }
        else {
            return '';
        }
    };
    return NowPlayingManager;
}());
exports.NowPlayingManager = NowPlayingManager;
var Device = /** @class */ (function () {
    function Device() {
    }
    return Device;
}());
//# sourceMappingURL=NowPlayingManager.js.map