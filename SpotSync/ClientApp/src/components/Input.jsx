/** @jsx jsx */
import { jsx } from "theme-ui";

export default (props) => {
  return (
    <input
      placeholder={props.placeholder}
      {...props}
      sx={{
        padding: "8px",
        bg: "darkgreyinput",
        border: "none",
        borderRadius: 20,
        fontSize: 17,
        width: "100%",
        ":focus": { outlineColor: "blue" },
      }}
    ></input>
  );
};
