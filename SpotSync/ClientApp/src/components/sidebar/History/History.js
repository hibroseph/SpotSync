import React from "react";
import QueueItem from "../Queue/QueueItem";
import { connect } from "react-redux";
import { getParty } from "../../../redux/reducers/reducers";
import Button from "../../shared/Button";
import { generateQueue } from "../../../api/party";

const History = (props) => {
  return props?.history?.length > 0 ? (
    <React.Fragment>
      {props.history.map((song) => {
        return <QueueItem title={song.name} artist={song.artist}></QueueItem>;
      })}
    </React.Fragment>
  ) : (
    <React.Fragment>
      <p>There currently is no history</p>
    </React.Fragment>
  );
};

const mapStateToProps = (state) => getParty(state);

export default connect(mapStateToProps, null)(History);
