import React, { useEffect, useState } from "react";
import styled from "styled-components";
import OutlinedContainer from "../../shared/OutlinedContainer";
import Title from "../../shared/Title";
import { getContributions, removeContribution as deleteContribution } from "../../../api/party";
import { error } from "../../shared/notify";
import Contribution from "../../shared/Contribution";
import Button from "../../shared/Button";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faTimes } from "@fortawesome/free-solid-svg-icons";

const $ContributionContainer = styled.div`
  display: flex;
  justify-content: center;
`;

const $StyledRemoveIcon = styled(FontAwesomeIcon)`
  margin: 0 5px 0 10px;

  &:hover {
    transform: scale(1.2);
  }
`;

const removeContribution = (partyCode, id, contributions, setContributions) => {
  deleteContribution(partyCode, id)
    .then(() => setContributions(contributions.filter((p) => p.contributionId != id)))
    .catch((err) => error(`Failed to remove contribution. Try again later.`));
};

export default ({ partyCode, showContributionsPopup, contributions, setContributions }) => {
  return (
    <OutlinedContainer>
      <Title>Your Music Contributions</Title>
      <$ContributionContainer>
        {contributions.length > 0 ? (
          <React.Fragment>
            {contributions.map((contribution, index) => (
              <Contribution key={`${contribution.id}_${index}`} name={contribution.name} type={contribution.type}>
                <$StyledRemoveIcon
                  icon={faTimes}
                  onClick={() => removeContribution(partyCode, contribution.contributionId, contributions, setContributions)}
                ></$StyledRemoveIcon>
              </Contribution>
            ))}
            <Contribution
              name="Add More Contributions"
              id="add_more"
              actOnContribution={() => {
                showContributionsPopup();
              }}
            ></Contribution>
          </React.Fragment>
        ) : (
          <div style={{ display: "flex", flexDirection: "column" }}>
            <p>You currently do not have any contributions to this party.</p>

            <Button onClick={() => showContributionsPopup()}>Add Some Music</Button>
          </div>
        )}
      </$ContributionContainer>
    </OutlinedContainer>
  );
};
