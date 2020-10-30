/** @jsx jsx */
import { jsx } from "theme-ui";
import React from "react";

export default (props) => {
  const [checked, setChecked] = React.useState(props.checked);

  const HandleChange = () => {
    setChecked(!checked);
  };
  return (
    <input
      sx={{ ":focus": { outlineColor: "blue" } }}
      type="checkbox"
      checked={checked}
      onChange={(target) => {
        HandleChange();
      }}
    ></input>
  );
};
