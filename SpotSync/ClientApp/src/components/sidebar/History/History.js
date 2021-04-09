import React from "react";
import QueueItem from "../Queue/QueueItem";
import { connect } from "react-redux";
import { getParty, getRealtimeConnection } from "../../../redux/reducers/reducers";

import { userLikesSong, userDislikesSong } from "../../../api/partyHub";

const History = ({ songFeelings, party, connection, dispatch }) => {
  return party?.history?.length > 0 ? (
    <React.Fragment>
      {party?.history.map((song, index) => {
        return (
          <QueueItem
            onLike={() => {
              userLikesSong(party.code, song.uri, connection, dispatch);
            }}
            onDislike={() => {
              userDislikesSong(party.code, song.uri, connection, dispatch);
            }}
            key={`${song.uri}_${index}`}
            title={song.name}
            artist={song.artist}
            feeling={songFeelings[song.uri]}
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
