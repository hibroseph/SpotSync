const u = require("umbrellajs");

export class PartyTabManager {
    private ContainerNames: Record<string, string>;
    constructor() {
        this.ContainerNames = {};
        this.AddOnClickListeners();
        this.AddContainerNameToDictionary();
    }

    private AddOnClickListeners = () => {
        u("#party-tabs").children("ul").on('click', event => {

            this.RemoveIsActiveFromAllTabs();

            this.AddIsActiveToSelectedElement(event.target);

            this.HideAllContainers();

            this.ShowSelectedContainer(event.target.dataset["page"]);
        })
    }

    private AddContainerNameToDictionary = () => {

        u("#party-tabs").children("ul").children("li").children().each((node, i) => {
            console.log(`adding ${u(node).data("page")}-container to container`)
            this.ContainerNames[u(node).data("page")] = `${u(node).data("page")}-container`;
        })
    }

    private RemoveIsActiveFromAllTabs = () => {
        u("#party-tabs").children("ul").children("li").each((node, i) => {
            u(node).removeClass("is-active");
        });
    }

    private AddIsActiveToSelectedElement = (targetNode) => {
        u(u(targetNode).parent()).addClass("is-active");
    }

    private HideAllContainers = () => {
        for (let key in this.ContainerNames) {
            console.log("key:" + key);
            u(`#${this.ContainerNames[key]}`).addClass("hidden");
        }
    }

    private ShowSelectedContainer = (containerName) => {
        u(`#${containerName}-container`).removeClass("hidden");
    }
}
