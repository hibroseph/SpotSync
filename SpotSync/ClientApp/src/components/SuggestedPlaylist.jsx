/** @jsx jsx */
import { jsx } from "theme-ui";
import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import SongWrapper from "./SongWrapper";
import Loading from "./Loading";
import { GetUserSuggestedSongs } from "../redux/Thunks";

export default (props) => {
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(GetUserSuggestedSongs());
  }, []);

  const songs = useSelector((state) => {
    return state.Playlist.createPlaylistSongs;
  });

  return (
    <div>
      {songs.length > 0 && (
        <div>
          <p sx={{ fontWeight: "bold" }}>Suggested Songs</p>
          <div sx={{ pr: "0px" }}>
            <div sx={{ display: "grid", gridTemplateColumns: "50% 40% 10%" }}>
              <p>Title</p>
              <p>Artist</p>
              <p sx={{ marginLeft: "auto" }}>Add</p>
            </div>
            {songs.map((song) => {
              return <SongWrapper key={song.trackUri} song={song}></SongWrapper>;
            })}
          </div>
        </div>
      )}

      {songs.length < 1 && <Loading sx={{ display: "flex", justifyContent: "center", py: "40px" }}></Loading>}
    </div>
  );
};
