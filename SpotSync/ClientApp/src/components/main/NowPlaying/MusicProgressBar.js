import React, { useState, useEffect } from "react";
import styled from "styled-components";

const $BarContainer = styled.div``;

const $Bar = styled.div`
  width: 80%;
  background-color: #f2f2f2;
  border-radius: 10px;
  height: 10px;
  position: absolute;
`;

const $Highlight = styled($Bar)`
  width: ${(props) => props.percentFull}%;
  background-color: #4497fb;
`;

export default ({ millisecond = 0, lengthOfSong }) => {
  const [trackTime, setTrackTime] = useState(millisecond);

  const updateSlider = (trackTime, setTrackTime, lengthOfSong) => {
    let tracky = trackTime + 1000;
    setTrackTime(tracky);
    console.log((trackTime / lengthOfSong) * 100);
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
    <$BarContainer>
      <div>
        <$Bar></$Bar>
        <$Highlight percentFull={trackTime != 0 ? (trackTime / lengthOfSong) * 100 : 0}></$Highlight>
      </div>
    </$BarContainer>
  );
};
