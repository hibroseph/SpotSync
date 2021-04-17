export const SHOW_ARTIST_VIEW = "show_artist_view";

export const showArtistView = (artist) => {
  console.log("action for showing artist view");
  return {
    type: SHOW_ARTIST_VIEW,
    artist,
  };
};
