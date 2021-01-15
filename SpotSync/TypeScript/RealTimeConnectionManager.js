"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var signalR = require("@microsoft/signalr");
/*
 * This class handles all the client side logic necessary to communicate with the server over SignalR
 * */
var RealTimeConnectionManager = /** @class */ (function () {
    function RealTimeConnectionManager() {
        console.log("We are in the constructor for realtime connection manager");
    }
    RealTimeConnectionManager.prototype.connectToParty = function (partyCode) {
        console.log("We are connecting to party");
        RealTimeConnectionManager.connection = new signalR.HubConnectionBuilder().withUrl("/partyhub").build();
        RealTimeConnectionManager.connection.start().then(function () {
            console.log("connection has started");
            RealTimeConnectionManager.connection.invoke("ConnectToParty", partyCode);
        });
    };
    RealTimeConnectionManager.prototype.getConnection = function () {
        return RealTimeConnectionManager.connection;
    };
    RealTimeConnectionManager.prototype.webPlayerInitialized = function (device_id) {
        RealTimeConnectionManager.connection.invoke("WebPlayerInitialized", device_id);
    };
    return RealTimeConnectionManager;
}());
exports.RealTimeConnectionManager = RealTimeConnectionManager;
//# sourceMappingURL=RealTimeConnectionManager.js.map