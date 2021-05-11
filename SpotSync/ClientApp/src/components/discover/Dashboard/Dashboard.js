import React, { useEffect, useState } from "react";
import styled from "styled-components";
import Contributions from "./Contributions";

export default ({ partyCode, showContributionsPopup, contributions, setContributions }) => {
  return (
    <Contributions
      showContributionsPopup={showContributionsPopup}
      partyCode={partyCode}
      contributions={contributions}
      setContributions={setContributions}
    ></Contributions>
  );
};
