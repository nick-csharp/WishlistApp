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


    this.goToDebugger = this.goToDebugger.bind(this);
    this.callAPI = this.callAPI.bind(this);
  }

  componentDidMount() {
    
  }

  async goToDebugger() {
    debugger;
    var token = await this.props.getAccessToken();
    console.log(token);
  }

  async callAPI() {

    var token = await this.props.getAccessToken();
    debugger;
    var headers = new Headers();
    var bearer = "Bearer " + token.accessToken;
    headers.append("Authorization", bearer);
    var options = {
      method: "GET",
      headers: headers
    };

    fetch("api/authtest/fgsfds", options)
      .then(function (response) {
        debugger;
      })
  }

  render() {
    
    return (
      <AuthContext.Provider value={this.props}>
        <Layout>
          <Router>
            <Whanau path='/' />
            <Wishlist path='/person/:personId/wishlist' />
          </Router>


          <div>
            <span>Authorised: {this.props.isAuthenticated.toString()}</span>
            <br />
            <button onClick={() => this.props.login()}>Sign in</button>
            <button onClick={() => this.props.logout()}>Sign out</button>
            <button onClick={() => this.callAPI()}>Call API</button>
            <br />
            <button onClick={() => this.goToDebugger()}>Debugger</button>
          </div>

        </Layout>
      </AuthContext.Provider>
    );
  }
}

export const App = withAuth(RootApp);
