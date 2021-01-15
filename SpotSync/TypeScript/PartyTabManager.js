"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var u = require("umbrellajs");
var PartyTabManager = /** @class */ (function () {
    function PartyTabManager() {
        var _this = this;
        this.AddOnClickListeners = function () {
            u("#party-tabs").children("ul").on('click', function (event) {
                _this.RemoveIsActiveFromAllTabs();
                _this.AddIsActiveToSelectedElement(event.target);
                _this.HideAllContainers();
                _this.ShowSelectedContainer(event.target.dataset["page"]);
            });
        };
        this.AddContainerNameToDictionary = function () {
            u("#party-tabs").children("ul").children("li").children().each(function (node, i) {
                console.log("adding " + u(node).data("page") + "-container to container");
                _this.ContainerNames[u(node).data("page")] = u(node).data("page") + "-container";
            });
        };
        this.RemoveIsActiveFromAllTabs = function () {
            u("#party-tabs").children("ul").children("li").each(function (node, i) {
                u(node).removeClass("is-active");
            });
        };
        this.AddIsActiveToSelectedElement = function (targetNode) {
            u(u(targetNode).parent()).addClass("is-active");
        };
        this.HideAllContainers = function () {
            for (var key in _this.ContainerNames) {
                console.log("key:" + key);
                u("#" + _this.ContainerNames[key]).addClass("hidden");
            }
        };
        this.ShowSelectedContainer = function (containerName) {
            u("#" + containerName + "-container").removeClass("hidden");
        };
        this.ContainerNames = {};
        this.AddOnClickListeners();
        this.AddContainerNameToDictionary();
    }
    return PartyTabManager;
}());
exports.PartyTabManager = PartyTabManager;
//# sourceMappingURL=PartyTabManager.js.map