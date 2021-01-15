"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var u = require("umbrellajs");
var ModalManager = /** @class */ (function () {
    function ModalManager(signalRConnection) {
        this.connection = signalRConnection;
    }
    ModalManager.prototype.setUpSignalRListeners = function () {
        var _this = this;
        this.connection.on("ExplicitSong", function (message) {
            _this.CreateAndShowModal("Explicit Content", "<p>" + message + "</p>", "<a onclick=\"Spotibro.CloseModal()\" class=\"card-footer-item\">Close</a>");
        });
        this.connection.on("ConnectSpotify", function (msg) {
            //console.log("Users Spotify is disconnected");
            u("#card-content").html();
            u(".modal").addClass("is-active");
            _this.CreateAndShowModal("Where is your Spotify?", "<p> It looks like your Spotify got disconnected from us.This might be because your Spotify app was closed.Make sure your Spotify app is open on whatever device you want to listen through and start playing a song and press the button below to see if we can find your Spotify </p>", "<a onclick=\"Spotibro.CheckForActiveSpotifyConnection()\" class=\"card-footer-item\">Sync</a>", "<a onclick=\"Spotibro.CloseModal()\" class=\"card-footer-item\">Close</a>");
        });
    };
    ModalManager.prototype.CreateAndShowModal = function (htmlTitle, htmlMessage, button1Html, button2Html) {
        if (button2Html === void 0) { button2Html = undefined; }
        u("#card-header").html(htmlTitle);
        u("#card-content").html(htmlMessage);
        u("#card-footer").empty();
        u("#card-footer").append(button1Html);
        if (button2Html != undefined) {
            u("#card-footer").append(button2Html);
        }
        u(".modal").addClass("is-active");
    };
    return ModalManager;
}());
exports.ModalManager = ModalManager;
//# sourceMappingURL=ModalManager.js.map