/** @jsx jsx */
import { jsx } from "theme-ui";
import "./Loading.css";

export default (props) => {
  return (
    <div {...props}>
      <div sx={{ color: "blue" }} className="la-ball-beat">
        <div></div>
        <div></div>
        <div></div>
      </div>
    </div>
  );
};
