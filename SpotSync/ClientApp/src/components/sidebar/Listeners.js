import React from "react";
import { connect } from "react-redux";
import Subtitle from "../shared/Subtitle";
import { getHost, getListeners } from "../../redux/reducers/reducers";
import Bubble from "../shared/Bubble";
import styled from "styled-components";

const $listener = styled.div`
  display: flex;
  align-items: center;
  width: 90%;
  height: 45px;
`;

const Listeners = ({ listeners, host }) => {
  return (
    <React.Fragment>
      {listeners != undefined ? (
        listeners.map((name) => {
          return (
            <$listener key={name}>
              <Subtitle>{name}</Subtitle>
              {name == host ? <Bubble color="#e2b727">Host</Bubble> : null}
            </$listener>
          );
        })
      ) : (
        <Subtitle>There are no listeners</Subtitle>
      )}
    </React.Fragment>
  );
};

const mapStateToProps = (state) => {
  return {
    host: getHost(state),
    listeners: getListeners(state),
  };
};
export default connect(mapStateToProps, null)(Listeners);
