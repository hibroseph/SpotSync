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
import OutlinedContainer from "../shared/OutlinedContainer";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faTimes } from "@fortawesome/free-solid-svg-icons";

const $ContributionContainer = styled.div`
  display: flex;
  justify-content: center;
  flex-wrap: wrap;
`;

const $StyledPopup = styled(Popup)`
  width: 70%;
`;

const $PopupNav = styled.div`
  display: flex;
  justify-content: space-between;
`;

const addContribution = (contribution, contributions, setContributions, suggestedContributions, setSuggestedContributions) => {
  setContributions([...contributions, contribution]);
  setSuggestedContributions(suggestedContributions.filter((p) => p.id != contribution.id));
};

const removeContribution = (contribution, contributions, setContributions, someContributions, setSomeContributions) => {
  console.log(`REMOVING`, contribution);
  setContributions(contributions.filter((p) => p.id != contribution.id));
  setSomeContributions([...someContributions, contribution]);
  console.log(someContributions);
};

const getSuggestedContributionsPls = (setIsLoading, suggestedContributions, setSuggestedContributions) => {
  setIsLoading(true);
  getSuggestedContributions(suggestedContributions.map((contribution) => contribution.id))
    .then((newSuggestedContributions) => {
      setSuggestedContributions([...suggestedContributions, ...newSuggestedContributions]);
      setIsLoading(false);
    })
    .catch((err) => {
      error("Unable to get suggested contributions");
      console.error(err);
    });
};

const sendContributionsToServer = (partyCode, contributons, contributionsToRemove, setPopup, setPartyInitalized, setGlobalContributions) => {
  addContributionsToParty(partyCode, contributons, contributionsToRemove)
    .catch((p) => error("Failed to add contributions to party. Try again later"))
    .finally((p) => {
      console.log("getting contributions");
      getContributions(partyCode)
        .then((realContributions) => {
          console.log("got contributions");
          console.log(realContributions);
          setGlobalContributions(realContributions);
        })
        .finally(() => {
          setPopup(null);
          setPartyInitalized(true);
        });
    });
};

export default ({ hideMusicContributionPopup, partyCode, setPopup, setPartyInitalized, setGlobalContributions }) => {
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
  const [newContributions, setNewContributions] = useState([]);
  const [contributionsToRemove, setContributionsToRemove] = useState([]);

  const [isLoading, setIsLoading] = useState(true);

  return (
    <$StyledPopup>
      <$PopupNav>
        <Title>Contribute Music</Title>
        <FontAwesomeIcon icon={faTimes} onClick={() => hideMusicContributionPopup()}></FontAwesomeIcon>
      </$PopupNav>
      <OutlinedContainer>
        <Subtitle>Suggested Contributions</Subtitle>
        <$ContributionContainer>
          {suggestedContributions?.map((contribution, index) => {
            return (
              <Contribution
                key={index}
                type={contribution.type}
                id={contribution.id}
                name={contribution.name}
                actOnContribution={(contribution) =>
                  addContribution(contribution, newContributions, setNewContributions, suggestedContributions, setSuggestedContributions)
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
        </$ContributionContainer>
      </OutlinedContainer>
      {isLoading && (
        <CenteredHorizontally>
          <Loader></Loader>
        </CenteredHorizontally>
      )}
      <OutlinedContainer>
        <Subtitle>Contributions</Subtitle>
        <$ContributionContainer>
          {contributions?.length == 0 && newContributions?.length == 0 && (
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
                  removeContribution(contribution, contributions, setContributions, contributionsToRemove, setContributionsToRemove)
                }
              ></Contribution>
            );
          })}
          {newContributions?.map((contribution, index) => {
            return (
              <Contribution
                key={index}
                type={contribution.type}
                id={contribution.id}
                name={contribution.name}
                actOnContribution={(contribution) =>
                  removeContribution(contribution, newContributions, setNewContributions, suggestedContributions, setSuggestedContributions)
                }
              ></Contribution>
            );
          })}
        </$ContributionContainer>
      </OutlinedContainer>
      <CenteredHorizontally>
        {newContributions?.length > 0 || contributionsToRemove?.length > 0 ? (
          <$Button
            onClick={() =>
              sendContributionsToServer(partyCode, newContributions, contributionsToRemove, setPopup, setPartyInitalized, setGlobalContributions)
            }
          >
            Update Contributions
          </$Button>
        ) : (
          <$Button
            onClick={() => {
              getContributions(partyCode)
                .then((realContributions) => {
                  console.log("got contributions");
                  console.log(realContributions);
                  setGlobalContributions(realContributions);
                })
                .finally(() => {
                  setPopup(null);
                  setPartyInitalized(true);
                });
            }}
          >
            Just Listen
          </$Button>
        )}
      </CenteredHorizontally>
    </$StyledPopup>
  );
};
