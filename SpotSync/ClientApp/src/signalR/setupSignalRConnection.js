import { JsonHubProtocol, HubConnectionState, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { realTimeConnectionEstablished } from "../redux/actions/signalr";
import {
  updateQueue,
  updateHistory,
  updateSong,
  partyJoined,
  updateCurrentSong,
  listenerJoined,
  listenerLeft,
  updateTrackVotes,
  updatePositionInSong,
} from "../redux/actions/party";
import notify from "../components/shared/notify";
const startSignalRConnection = async (connection, dispatch) => {
  try {
    await connection.start();
    console.assert(connection.state === HubConnectionState.Connected);
    dispatch(realTimeConnectionEstablished(connection));
  } catch (err) {
    console.assert(connection.state === HubConnectionState.Disconnected);
    console.error("SignalR Connection Error: ", err);
    setTimeout(() => startSignalRConnection(connection, dispatch), 5000);
  }
};

// Set up a SignalR connection to the specified hub URL, and actionEventMap.
// actionEventMap should be an object mapping event names, to eventHandlers that will
// be dispatched with the message body.
export const setupSignalRConnection = (connectionHub, actionEventMap = {}, getAccessToken) => (dispatch, getState) => {
  const options = {
    //accessTokenFactory: () => getAccessToken(getState),
  };
  // create the connection instance
  // withAutomaticReconnect will automatically try to reconnect
  // and generate a new socket connection if needed
  const connection = new HubConnectionBuilder()
    .withUrl(connectionHub, options)
    .withAutomaticReconnect()
    .withHubProtocol(new JsonHubProtocol())
    .configureLogging(LogLevel.Information)
    .build();

  // Note: to keep the connection open the serverTimeout should be
  // larger than the KeepAlive value that is set on the server
  // keepAliveIntervalInMilliseconds default is 15000 and we are using default
  // serverTimeoutInMilliseconds default is 30000 and we are using 60000 set below
  connection.serverTimeoutInMilliseconds = 60000;

  // re-establish the connection if connection dropped
  connection.onclose((error) => {
    console.assert(connection.state === HubConnectionState.Disconnected);
    console.log("Connection closed due to error. Try refreshing this page to restart the connection", error);
  });

  connection.onreconnecting((error) => {
    console.assert(connection.state === HubConnectionState.Reconnecting);
    console.log("Connection lost due to error. Reconnecting.", error);
  });

  connection.onreconnected((connectionId) => {
    console.assert(connection.state === HubConnectionState.Connected);
    console.log("Connection reestablished. Connected with connectionId", connectionId);
  });

  startSignalRConnection(connection, dispatch);

  connection.on("UpdateParty", (res) => {});

  connection.on("NewListener", (listener) => {
    notify(`${listener} joined your party`);
    dispatch(listenerJoined(listener));
  });

  connection.on("UpdateQueue", (queue) => {
    dispatch(updateQueue(queue));
  });

  connection.on("InitialPartyLoad", (currentSong, history, queue, details) => {
    dispatch(updateCurrentSong(currentSong.song));
    dispatch(updateQueue(queue));
    dispatch(updateHistory(history));
    dispatch(partyJoined(details.partyCode, details.listeners, details.host));
    dispatch(updatePositionInSong(currentSong.position));
  });

  connection.on("ListenerLeft", (name) => {
    notify(`${name} left your party`);
    dispatch(listenerLeft(name));
  });

  connection.on("UpdatePartyView", (currentSong, history, queue, details) => {
    dispatch(updateQueue(queue));
    dispatch(updateHistory(history));
    dispatch(updateCurrentSong(currentSong.song));
    dispatch(updatePositionInSong(details.position));
  });

  connection.on("ExplicitSong", (res) => {
    const eventHandler = actionEventMap[res.eventType];
    eventHandler && dispatch(eventHandler(res));
  });

  connection.on("UpdateTrackVotes", (trackVotes) => {
    dispatch(updateTrackVotes(trackVotes));
  });

  connection.on("InitializeWebPlayer", (res) => {
    const eventHandler = actionEventMap[res.eventType];
    eventHandler && dispatch(eventHandler(res));
  });

  connection.on("ConnectSpotify", (res) => {
    const eventHandler = actionEventMap[res.eventType];
    eventHandler && dispatch(eventHandler(res));
  });

  connection.on("UpdateSong", (updateSongObj) => {
    dispatch(updateSong(updateSongObj.song, updateSongObj.position));
  });

  return connection;
};
