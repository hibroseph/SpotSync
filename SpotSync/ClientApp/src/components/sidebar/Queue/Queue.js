import React from "react";
import QueueItem from "./QueueItem";
import { connect } from "react-redux";
import { getParty, getRealtimeConnection, getTrackVotes } from "../../../redux/reducers/reducers";
import Button from "../../shared/Button";
import { generateQueue } from "../../../api/party";
import { userLikesSong, userDislikesSong, nukeQueue } from "../../../api/partyHub";
import Subtitle from "../../shared/Subtitle";
import CenteredHorizontally from "../../shared/CenteredHorizontally";
import toast from "../../shared/notify";
import styled from "styled-components";

const $NukeButton = styled(Button)`
  background-color: #bd3939;
  margin: 30px;
`;

const Queue = ({ party, connection, trackVotes = {}, songFeelings = {}, dispatch, isHost }) => {
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
            trackVotes={song.id in trackVotes ? trackVotes[song.id] : 0}
            key={`${song.id}_${index}`}
            title={song.name}
            artists={song.artists}
            feeling={songFeelings[song.id] ?? -1}
          ></QueueItem>
        );
      })}
      {isHost && <$NukeButton onClick={() => nukeQueue(party.code, connection)}>Nuke Queue</$NukeButton>}
    </React.Fragment>
  ) : party?.nowPlaying?.id != undefined ? (
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
  return { party: getParty(state), connection: getRealtimeConnection(state).connection, trackVotes: getTrackVotes(state) };
};

export default connect(mapStateToProps, null)(Queue);
