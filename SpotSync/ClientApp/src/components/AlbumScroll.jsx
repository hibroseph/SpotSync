import React from "react";
import { Flex } from "rebass";
import ImageContainer from "./ImageContainer";

const albums = [
  { id: "1", art: "https://i.pinimg.com/originals/b4/75/00/b4750046d94fed05d00dd849aa5f0ab7.jpg", listeners: 210 },
  { id: "2", art: "https://99designs-blog.imgix.net/blog/wp-content/uploads/2017/12/attachment_68585523.jpg", listeners: 22 },
  { id: "3", art: "https://humanhuman.imgix.net/articles/49/ben_khan_1000.jpg", listeners: 3 },
  {
    id: "4",
    art: "https://i.cbc.ca/1.4574015.1520953045!/fileImage/httpImage/image.jpg_gen/derivatives/original_780/daniel-caesar.jpg",
    listeners: 57,
  },
];

export default (props) => {
  return (
    <Flex flexDirection="row" overflow="auto">
      {albums.length !== 0 &&
        albums.map((album) => {
          return <ImageContainer key={album.id} imgUrl={album.art} listeners={album.listeners}></ImageContainer>;
        })}
      {albums.length === 0 && <p>There are no current parties. You should start one.</p>}
    </Flex>
  );
};
