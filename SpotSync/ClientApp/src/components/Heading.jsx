/** @jsx jsx */
import { jsx } from "theme-ui";

export default (props) => {
  return (
    <h2
      {...props}
      sx={{
        mb: "5px",
        mt: "0px",
      }}
    >
      {props.children}
    </h2>
  );
};
