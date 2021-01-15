const u = require("umbrellajs");

export class NotificationManager {
    ShowMessage(message: string, duration: number = 5000) {
        let id = this.stringGen(5);
        u("#user-notifications").append(this.GetNotification(message, id))
        setTimeout(() => {
            u(`#${id}`).remove();
        }, duration);
    }

    private GetNotification(message: string, id: string) {
        return `<div class="notification is-info" id="${id}">
                    ${message}
                </div>`
    }

    private stringGen(len) {
        var text = "";

        var charset = "abcdefghijklmnopqrstuvwxyz";

        for (var i = 0; i < len; i++)
            text += charset.charAt(Math.floor(Math.random() * charset.length));

        return text;
    }
}