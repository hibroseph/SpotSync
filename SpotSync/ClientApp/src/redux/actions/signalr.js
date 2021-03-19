export const REALTIME_CONNECTION_ESTABLISHED = "realtime_connection_established";

export const realTimeConnectionEstablished = (connection) => {
  return {
    type: REALTIME_CONNECTION_ESTABLISHED,
    connection,
  };
};
