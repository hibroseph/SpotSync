import React, { useEffect, useState } from "react";
import styled from "styled-components";
import Contributions from "./Contributions";

export default ({ partyCode, partyInitalized, showContributionsPopup }) => {
  return <Contributions showContributionsPopup={showContributionsPopup} partyInitalized={partyInitalized} partyCode={partyCode}></Contributions>;
};
