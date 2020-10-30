/** @jsx jsx */
import { jsx } from "theme-ui";
import Checkbox from "./Checkbox";
import { TOGGLE_CREATE_PLAYLIST_SONG } from "../redux/ActionTypes";
import { useDispatch } from "react-redux";
/*
Contract
{ name: "My Lucky 3", artist: "Night Owl", checked: true },
*/
export default (props) => {
  const dispatch = useDispatch();

  return (
    <div sx={{ display: "grid", gridTemplateColumns: "50% 40% 10%" }}>
      <p>{props.song.title}</p>
      <p>{props.song.artist}</p>
      <Checkbox
        handleCheck={(checked) => {
          console.log("handling check");
          dispatch({ type: TOGGLE_CREATE_PLAYLIST_SONG, payload: { ...props.song, checked } });
        }}
        sx={{ mt: "1.5em", marginLeft: "auto" }}
        checked={props.song.checked}
      ></Checkbox>
    </div>
  );
};
