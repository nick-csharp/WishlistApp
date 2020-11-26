import React, { Component } from "react";
import { Router } from "@reach/router";

import "./custom.css";
import { Layout } from "./components/Layout";
import Whanau from "./components/Whanau";
import { Wishlist } from "./components/Wishlist";
import {
  withAuth,
  MsalAuthContext,
} from "./components/msal/MsalAuthProvider";

class RootApp extends Component {
  static contextType = MsalAuthContext;
  constructor(props) {
    super(props);
    this.state = {
      data: "unloaded",
      isLoading: true,
    };
  }

  render() {
    var authContext = this.context;

    if (!authContext.isAuthenticated) {
      return <button onClick={() => authContext.login()}>Login</button>;
    }

    return (
      <Layout>
        <Router>
          <Whanau path="/" />
          <Wishlist path="/person/:personId/wishlist" />
        </Router>
      </Layout>
    );
  }
}

export default withAuth(RootApp);