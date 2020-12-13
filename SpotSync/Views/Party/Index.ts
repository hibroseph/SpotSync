export class PartyMenu {
    private uiElement: HTMLElement;
    private state = { hidden: true };

    constructor(uiElement: HTMLElement) {
        this.uiElement = uiElement;
        this.setUpListeners(this.uiElement);
    }

    private setUpListeners(rootElement: HTMLElement) {
        console.log("Setting up listeners for PartyMenu");
        rootElement.querySelector("#").addEventListener("click", () => {

        })
    }
}