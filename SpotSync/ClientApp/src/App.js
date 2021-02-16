import React from "react";
import Navigation from "./components/navigation/Navigation";
import MainContent from "./components/main/MainContent";

function App(props) {
  console.log("Hello World");
  return (
    <div>
      <Navigation />
      <MainContent></MainContent>
    </div>
  );
}

export default App;
