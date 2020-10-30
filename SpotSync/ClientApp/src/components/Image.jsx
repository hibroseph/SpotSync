/** @jsx jsx */
import { jsx } from "theme-ui";

export default (props) => {
  return (
    <img
      {...props.sx}
      sx={{
        mx: 10,
        width: [200, 250],
      }}
      src={props.imgUrl}
      alt={props.alt}
    ></img>
  );
};
