"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var u = require("umbrellajs");
var NotificationManager = /** @class */ (function () {
    function NotificationManager() {
    }
    NotificationManager.prototype.ShowMessage = function (message, duration) {
        if (duration === void 0) { duration = 5000; }
        var id = this.stringGen(5);
        u("#user-notifications").append(this.GetNotification(message, id));
        setTimeout(function () {
            u("#" + id).remove();
        }, duration);
    };
    NotificationManager.prototype.GetNotification = function (message, id) {
        return "<div class=\"notification is-info\" id=\"" + id + "\">\n                    " + message + "\n                </div>";
    };
    NotificationManager.prototype.stringGen = function (len) {
        var text = "";
        var charset = "abcdefghijklmnopqrstuvwxyz";
        for (var i = 0; i < len; i++)
            text += charset.charAt(Math.floor(Math.random() * charset.length));
        return text;
    };
    return NotificationManager;
}());
exports.NotificationManager = NotificationManager;
//# sourceMappingURL=NotificationManager.js.map