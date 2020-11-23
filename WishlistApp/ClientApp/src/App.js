import React, { Component } from 'react';
import Whanau from './components/Whanau';
import { Wishlist } from './components/Wishlist';
import { Router } from "@reach/router";
import { withAuth } from "./components/msal/MsalAuthProvider";

import './custom.css'
import { Layout } from './components/Layout';
import { AuthContext } from './components/AuthContext';


class RootApp extends Component {

  constructor(props) {
    super(props);
    this.state = { data: "unloaded", isLoading: true };
  }

  render() {
    
    return (
      <AuthContext.Provider value={this.props}>
        <Layout>
          <Router>
            <Whanau path='/' />
            <Wishlist path='/person/:personId/wishlist' />
          </Router>
        </Layout>
      </AuthContext.Provider>
    );
  }
}

export const App = withAuth(RootApp);
