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
        fetch("/api/user/getactivedevice")
            .then(function (response) { return response.json(); })
            .then(function (device) {
            console.log("the users device is " + device.deviceName);
            _this.hideLoader();
            _this.displayDeviceInPopUp(device.deviceName);
        });
    };
    NowPlayingManager.prototype.hideLoader = function () {
        u("#device-loader").removeClass("is-active");
    };
    NowPlayingManager.prototype.displayDeviceInPopUp = function (deviceName) {
        u("#active-device").append(this.createHtmlForDevice(deviceName));
    };
    NowPlayingManager.prototype.createHtmlForDevice = function (deviceName) {
        return "<p class=\"spotibro-text\">" + deviceName + "</p>";
    };
    return NowPlayingManager;
}());
exports.NowPlayingManager = NowPlayingManager;
//# sourceMappingURL=NowPlayingManager.js.map