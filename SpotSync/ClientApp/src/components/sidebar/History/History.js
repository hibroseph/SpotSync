import React from "react";
import QueueItem from "../Queue/QueueItem";
import { connect } from "react-redux";
import { getParty, getRealtimeConnection } from "../../../redux/reducers/reducers";

import { userLikesSong, userDislikesSong } from "../../../api/partyHub";

const History = ({ party, connection, dispatch }) => {
  return party?.history?.length > 0 ? (
    <React.Fragment>
      {party?.history.map((song, index) => {
        return (
          <QueueItem
            onLike={() => {
              userLikesSong(party.code, song.id, connection, dispatch);
            }}
            onDislike={() => {
              userDislikesSong(party.code, song.id, connection, dispatch);
            }}
            key={`${song.id}_${index}`}
            title={song.name}
            artists={song.artists}
          ></QueueItem>
        );
      })}
    </React.Fragment>
  ) : (
    <React.Fragment>
      <p>There currently is no history</p>
    </React.Fragment>
  );
};

const mapStateToProps = (state) => {
  return { party: getParty(state), connection: getRealtimeConnection(state).connection };
};

export default connect(mapStateToProps, null)(History);
