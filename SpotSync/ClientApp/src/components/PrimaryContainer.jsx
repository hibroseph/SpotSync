/** @jsx jsx */
import { jsx } from "theme-ui";

export default (props) => {
  return (
    <div
      {...props}
      sx={{
        display: "flex",
        bg: "lightgrey",
        p: "20px",
        borderRadius: 25,
        m: "5px",
      }}
    >
      {props.children}
    </div>
  );
};
