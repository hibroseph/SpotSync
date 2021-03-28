import React from "react";
import QueueItem from "./QueueItem";
import { connect } from "react-redux";
import { getParty } from "../../../redux/reducers/reducers";
import Button from "../../shared/Button";
import { generateQueue } from "../../../api/party";

const Queue = (props) => {
  return props?.queue?.length > 0 ? (
    <React.Fragment>
      {props.queue.map((song) => {
        return <QueueItem key={song.uri} title={song.name} artist={song.artist}></QueueItem>;
      })}
    </React.Fragment>
  ) : (
    <React.Fragment>
      <Button onClick={() => generateQueue(props.code)}>Generate Playlist</Button>
    </React.Fragment>
  );
};

const mapStateToProps = (state) => getParty(state);

export default connect(mapStateToProps, null)(Queue);
