import React, { Component } from 'react';
import { Whanau } from './components/Whanau';
import { Wishlist } from './components/Wishlist';
import { Router } from "@reach/router";
import { withAuth } from "./components/msal/MsalAuthProvider";

import './custom.css'
import { Layout } from './components/Layout';
import { AuthTest } from './components/AuthTest';

class RootApp extends Component {

  constructor(props) {
    super(props);
    this.state = { data: "unloaded", isLoading: true };
  }

  componentDidMount() {
    debugger;
  }

  render () {
    return (
      <Layout>
        <Router>
          <Whanau path='/' />
          <AuthTest path='/authtest' />
          <Wishlist path='/person/:personId/wishlist' />
        </Router>
      </Layout>
    );
  }
}

export const App = withAuth(RootApp);
