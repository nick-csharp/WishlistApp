import React, { Component } from "react";
import Loading from "./Loading";
import { MsalAuthContext } from "./msal/MsalAuthProvider";
import { navigate } from "@reach/router";

export default class AuthReply extends Component {
  static contextType = MsalAuthContext;

  render() {
    if (this.context.isAuthenticated) {
      return (
        <div>
          <span>Logged in!</span>
          <br />
          <button onClick={(e) => navigate("/whanau")}>
            Click here to go to Whanau
          </button>
        </div>
      );
    } else {
      return <Loading />;
    }
  }
}
