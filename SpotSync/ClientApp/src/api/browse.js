export const searchArtist = (artistId) => {
  return fetch(`/api/browse/searchartist?artistId=${artistId}`)
    .then((res) => res.json())
    .then((artist) => artist);
};
