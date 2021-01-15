const signalR = require("@microsoft/signalr")
import { NowPlayingManager } from '../TypeScript/NowPlayingManager'
import { PartyTabManager } from "../TypeScript/PartyTabManager"

/*
 * This class handles all the client side logic necessary to communicate with the server over SignalR
 * */
export class RealTimeConnectionManager {
    private static connection: any;

    constructor() {
        console.log("We are in the constructor for realtime connection manager");
    }

    public connectToParty(partyCode: string) {
        console.log("We are connecting to party")
        RealTimeConnectionManager.connection = new signalR.HubConnectionBuilder().withUrl("/partyhub").build();
        RealTimeConnectionManager.connection.start().then(() => {
            console.log("connection has started")
            RealTimeConnectionManager.connection.invoke("ConnectToParty", partyCode);
        });
    }

    public getConnection() {
        return RealTimeConnectionManager.connection;
    }

    public webPlayerInitialized(device_id) {
        RealTimeConnectionManager.connection.invoke("WebPlayerInitialized", device_id);
    }

}