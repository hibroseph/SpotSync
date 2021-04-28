export const SHOW_ARTIST_VIEW = "show_artist_view";

export const showArtistView = (artist) => {
  return {
    type: SHOW_ARTIST_VIEW,
    artist,
  };
};
