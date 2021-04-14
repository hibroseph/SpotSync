import React from "react";
import QueueItem from "./QueueItem";
import { connect } from "react-redux";
import { getParty, getRealtimeConnection } from "../../../redux/reducers/reducers";
import Button from "../../shared/Button";
import { generateQueue } from "../../../api/party";
import { userLikesSong, userDislikesSong } from "../../../api/partyHub";
import Subtitle from "../../shared/Subtitle";
import CenteredHorizontally from "../../shared/CenteredHorizontally";
import toast from "../../../api/notify";

const Queue = ({ party, connection, songFeelings = {}, dispatch }) => {
  return party?.queue?.length > 0 ? (
    <React.Fragment>
      {party.queue.map((song, index) => {
        return (
          <QueueItem
            onLike={() => {
              userLikesSong(party.code, song.id, connection, dispatch);
              toast(`We will play more songs like ${song.name}`);
            }}
            onDislike={() => {
              userDislikesSong(party.code, song.id, connection, dispatch);
              toast(`We will play less songs like ${song.name}`);
            }}
            key={`${song.id}_${index}`}
            title={song.name}
            artists={song.artists}
            feeling={songFeelings[song.id]}
          ></QueueItem>
        );
      })}
    </React.Fragment>
  ) : party?.nowPlaying != undefined ? (
    <CenteredHorizontally>
      <Subtitle>A new queue will be generated from your liked songs next song.</Subtitle>
    </CenteredHorizontally>
  ) : (
    <React.Fragment>
      <Button onClick={() => generateQueue(party.code)}>Generate Playlist</Button>
    </React.Fragment>
  );
};

const mapStateToProps = (state) => {
  return { party: getParty(state), connection: getRealtimeConnection(state).connection };
};

export default connect(mapStateToProps, null)(Queue);
