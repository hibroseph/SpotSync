/** @jsx jsx */
import { jsx } from "theme-ui";

export default (props) => {
  return (
    <a
      {...props}
      sx={{
        borderRadius: 21,
        px: 20,
        py: 10,
        ":focus": { outlineColor: "blue" },
        bg: "blue",
        border: "none",
        color: "white",
        textDecoration: "none",
      }}
    >
      {props.children}
    </a>
  );
};
