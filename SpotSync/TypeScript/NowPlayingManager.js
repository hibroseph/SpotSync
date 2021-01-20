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
    NowPlayingManager.prototype.getUsersAvailableDevices = function () {
    };
    return NowPlayingManager;
}());
exports.NowPlayingManager = NowPlayingManager;
//# sourceMappingURL=NowPlayingManager.js.map