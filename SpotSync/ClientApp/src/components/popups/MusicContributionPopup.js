import React, { useEffect, useState } from "react";
import Popup from "../shared/Popup";
import { getSuggestedContributions } from "../../api/user";
import Loader from "../shared/Loader";
import Subtitle from "../shared/Subtitle";
import { error } from "../shared/notify";
import Contribution from "../shared/Contribution";
import styled from "styled-components";
import Text from "../shared/Text";
import Title from "../shared/Title";
import CenteredHorizontally from "../shared/CenteredHorizontally";
import $Button from "../shared/Button";
import { addContributionsToParty } from "../../api/party";
import { getContributions } from "../../api/party";

const $contributionContainer = styled.div`
  display: flex;
  justify-content: center;
  flex-wrap: wrap;
`;

const addContribution = (contribution, contributions, setContributions, suggestedContributions, setSuggestedContributions) => {
  setContributions([...contributions, contribution]);
  setSuggestedContributions(suggestedContributions.filter((p) => p.id != contribution.id));
};

const removeContribution = (contribution, contributions, setContributions, suggestedContributions, setSuggestedContributions) => {
  setContributions(contributions.filter((p) => p.id != contribution.id));
  setSuggestedContributions([...suggestedContributions, contribution]);
};

const getSuggestedContributionsPls = (setIsLoading, suggestedContributions, setSuggestedContributions) => {
  setIsLoading(true);
  getSuggestedContributions()
    .then((newSuggestedContributions) => {
      setSuggestedContributions([...suggestedContributions, ...newSuggestedContributions]);
      setIsLoading(false);
    })
    .catch((err) => {
      error("Unable to get suggested contributions");
    });
};

const sendContributionsToServer = (partyCode, contributons, setPopup, setPartyInitalized) => {
  addContributionsToParty(partyCode, contributons)
    .catch((p) => error("Failed to add contributions to party. Try again later"))
    .finally((p) => {
      setPopup(null);
      setPartyInitalized(true);
    });
};

export default ({ hideMusicContributionPopup, partyCode, setPopup, setPartyInitalized }) => {
  useEffect(() => {
    getSuggestedContributionsPls(setIsLoading, suggestedContributions, setSuggestedContributions);
  }, []);

  useEffect(() => {
    if (partyCode != undefined) {
      getContributions(partyCode).then((contributons) => setContributions(contributons));
    }
  }, [partyCode]);

  const [suggestedContributions, setSuggestedContributions] = useState([]);
  const [contributions, setContributions] = useState([]);
  const [isLoading, setIsLoading] = useState(true);

  return (
    <Popup>
      <Title>Contribute Music</Title>
      <Subtitle>Suggested Contributions</Subtitle>
      <$contributionContainer>
        {suggestedContributions?.map((contribution, index) => {
          return (
            <Contribution
              key={index}
              type={contribution.type}
              id={contribution.id}
              name={contribution.name}
              actOnContribution={(contribution) =>
                addContribution(contribution, contributions, setContributions, suggestedContributions, setSuggestedContributions)
              }
            ></Contribution>
          );
        })}
        <Contribution
          id="load_more"
          name="Load More"
          actOnContribution={(contribution) => {
            getSuggestedContributionsPls(setIsLoading, suggestedContributions, setSuggestedContributions);
          }}
        ></Contribution>
      </$contributionContainer>
      {isLoading && (
        <CenteredHorizontally>
          <Loader></Loader>
        </CenteredHorizontally>
      )}
      <Subtitle>Contributions</Subtitle>
      <$contributionContainer>
        {contributions?.length == 0 && (
          <p>You currently are not contributing anything to this party. Click some items above to add some contributions.</p>
        )}
        {contributions?.map((contribution, index) => {
          return (
            <Contribution
              key={index}
              type={contribution.type}
              id={contribution.id}
              name={contribution.name}
              actOnContribution={(contribution) =>
                removeContribution(contribution, contributions, setContributions, suggestedContributions, setSuggestedContributions)
              }
            ></Contribution>
          );
        })}
      </$contributionContainer>
      <CenteredHorizontally>
        {contributions?.length > 0 ? (
          <$Button onClick={() => sendContributionsToServer(partyCode, contributions, setPopup, setPartyInitalized)}>Contribute</$Button>
        ) : (
          <$Button
            onClick={() => {
              hideMusicContributionPopup();
              setPartyInitalized(true);
            }}
          >
            Just Listen
          </$Button>
        )}
      </CenteredHorizontally>
    </Popup>
  );
};
