import React, { Component } from "react";
import { Router } from "@reach/router";

import "./custom.css";
import Layout from "./components/Layout";
import Whanau from "./components/Whanau";
import Wishlist from "./components/Wishlist";
import Landing from "./components/Landing";
import { withAuth } from "./components/msal/MsalAuthProvider";

class RootApp extends Component {
  render() {
    return (
      <Layout>
        <Router>
          <Landing path="/" />
          <Whanau path="/whanau" />
          <Wishlist path="/person/:personId/wishlist" />
        </Router>
      </Layout>
    );
  }
}

export default withAuth(RootApp);
