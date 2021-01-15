"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var u = require("umbrellajs");
var ListenerManager = /** @class */ (function () {
    function ListenerManager(signalRConnection) {
        this.connection = signalRConnection;
        this.SetUpSignalRListeners();
    }
    ListenerManager.prototype.SetUpSignalRListeners = function () {
        this.connection.on("NewListener", function (userName) {
            //console.log(userName)
            u("#listeners").append("<p>" + userName + "</p>");
        });
    };
    return ListenerManager;
}());
exports.ListenerManager = ListenerManager;
//# sourceMappingURL=ListenerManager.js.map