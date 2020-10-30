import React from "react";
import ReactDOM from "react-dom";
import "./index.css";
import App from "./App";
import * as serviceWorker from "./serviceWorker";
import { Provider } from "react-redux";
import { ThemeProvider } from "theme-ui";
import store from "./redux/Store";

const theme = {
  colors: {
    blue: "#45b3f1",
    lightgrey: "#f2f2f2",
    darkgreyinput: "#cacaca",
    darkgreytext: "#909090",
    background: "white",
  },
};

ReactDOM.render(
  <React.StrictMode>
    <ThemeProvider theme={theme}>
      <Provider store={store}>
        <App
          sx={{
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
          }}
        />
      </Provider>
    </ThemeProvider>
  </React.StrictMode>,
  document.getElementById("root")
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
