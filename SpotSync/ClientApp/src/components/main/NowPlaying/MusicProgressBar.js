import React, { useState, useEffect } from "react";
import styled from "styled-components";

const $BarContainer = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
`;

const $Bar = styled.div`
  width: 80%;
  background-color: #f2f2f2;
  border-radius: 10px;
  height: 10px;
`;

const $Highlight = styled($Bar)`
  background-color: #4497fb;
`;

const $TrackTime = styled.p`
  margin: 0 10px 0 10px;
  color: #b2b2b2;
  font-size: 12px;
`;

const convertMsToSongTime = (songTimeInMs) => {
  let time = new Date(songTimeInMs).toISOString();
  return time.slice(14, 19);
};

export default ({ millisecond = 0, lengthOfSong = 0 }) => {
  const [trackTime, setTrackTime] = useState(millisecond);
  const updateSlider = (trackTime, setTrackTime, lengthOfSong) => {
    setTrackTime(trackTime + 1000);
  };

  useEffect(() => {
    const timer = setTimeout(() => {
      updateSlider(trackTime, setTrackTime, lengthOfSong);
    }, 1000);
    return () => {
      clearTimeout(timer);
    };
  }, [trackTime]);

  useEffect(() => {
    setTrackTime(millisecond);
  }, [millisecond]);

  return (
    <div>
      <$BarContainer>
        <$TrackTime>{convertMsToSongTime(trackTime)}</$TrackTime>
        <$Bar>
          <$Highlight style={{ width: `${trackTime != 0 ? (trackTime / lengthOfSong) * 100 : 0}%` }}></$Highlight>
        </$Bar>
        <$TrackTime>{convertMsToSongTime(lengthOfSong)}</$TrackTime>
      </$BarContainer>
    </div>
  );
};
