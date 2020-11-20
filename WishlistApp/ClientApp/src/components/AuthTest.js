import React, { Component } from "react";
import { Loading } from "./Loading";
import { withAuth } from "./msal/MsalAuthProvider";

class RootAuthTest extends Component {
  constructor(props) {
    super(props);
    this.state = { data: "unloaded", isLoading: true };

    this.populateAuthTestData = this.populateAuthTestData.bind(this);
    this.goToDebugger = this.goToDebugger.bind(this);
    this.callAPI = this.callAPI.bind(this);
  }

  componentDidMount() {
    this.populateAuthTestData();
  }

  async populateAuthTestData() {
    //const resp = await this.props.getAccessToken();

    //const response = await fetch(`api/authtest/asdasdas`);
    //const data = await response.text();
    //debugger;
    //this.setState({ data: data, isLoading: false });
    this.setState({ isLoading: false });
  }

  async goToDebugger() {
    debugger;
    var token = await this.props.getAccessToken();
    console.log(token);
  }

  async callAPI() {

    var token = await this.props.getAccessToken();

    var headers = new Headers();
    var bearer = "Bearer " + token;
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

    if (this.state.isLoading) {
      return <Loading />
    }
    else {
      return (
        <div>
          <span>Authorised: {this.props.isAuthenticated.toString()}</span>
          <br/>
          <button onClick={() => this.props.onSignIn()}>Sign in</button>
          <button onClick={() => this.props.onSignOut()}>Sign out</button>
          <button onClick={() => this.callAPI()}>Call API</button>
          <br />
          <button onClick={() => this.goToDebugger()}>Debugger</button>
        </div>
      );
    }
  }
}

export const AuthTest = withAuth(RootAuthTest);
