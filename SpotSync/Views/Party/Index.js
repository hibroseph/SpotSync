"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var PartyMenu = /** @class */ (function () {
    function PartyMenu(uiElement) {
        this.state = { hidden: true };
        this.uiElement = uiElement;
        this.setUpListeners(this.uiElement);
    }
    PartyMenu.prototype.setUpListeners = function (rootElement) {
        console.log("Setting up listeners for PartyMenu");
        rootElement.querySelector("#").addEventListener("click", function () {
        });
    };
    return PartyMenu;
}());
exports.PartyMenu = PartyMenu;
//# sourceMappingURL=Index.js.map