/** @jsx jsx */
import { jsx } from "theme-ui";
import { useSelector } from "react-redux";
import Button from "./Button";
import PrimaryContainer from "./PrimaryContainer";

export default (props) => {
  const loggedIn = useSelector((state) => state.Account.loggedIn);

  return (
    <PrimaryContainer sx={{ width: "99%", justifyContent: "space-between", p: "10px" }}>
      {!loggedIn && <Button href="https://localhost:44346/api/account/login">Login</Button>}
      {loggedIn && <Button href="https://localhost:44346/api/account/logout">Logout</Button>}
    </PrimaryContainer>
  );
};
