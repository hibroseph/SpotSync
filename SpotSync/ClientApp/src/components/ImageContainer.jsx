/** @jsx jsx */
import { jsx } from "theme-ui";
import Image from "./Image";

export default (props) => {
  return (
    <div
      sx={{
        display: "flex",
        m: "5px",
        flexDirection: "column",
        alignItems: "center",
      }}
    >
      <Image imgUrl={props.imgUrl}></Image>
      {props.listeners != null && <p>{props.listeners} listeners</p>}
    </div>
  );
};
