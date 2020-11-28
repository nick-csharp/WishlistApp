import React, { Component } from "react";
import Loading from "./Loading";
import { MsalAuthContext } from "./msal/MsalAuthProvider";
import { navigate } from "@reach/router";

export default class Landing extends Component {
  static contextType = MsalAuthContext;
  constructor(props) {
    super(props);
    this.state = { isLoggingIn: false };

    this.handleLogin = this.handleLogin.bind(this);
  }

  handleLogin(e) {
    e.preventDefault();
    this.setState({ isLoggingIn: true });
    this.context.login();
  }

  render() {
    if (this.context.isLoading) {
      return <Loading />;
    }

    if (this.context.isAuthenticated) {
      navigate("/whanau");
    }

    return (
      <div className="container mt-4">
        <div className="row justify-content-center">
          <div className="card text-center">
            <div className="card-header">
              <h1 className="font-weight-light text-center">
                Welcome to the Wishlists&nbsp;app!
              </h1>
            </div>
            <div className="card-body">
              {this.state.isLoggingIn ? (
                <Loading />
              ) : (
                <button
                  type="button"
                  className="btn btn-success"
                  onClick={(e) => this.handleLogin(e)}
                >
                  Log in
                </button>
              )}
            </div>
          </div>
        </div>
      </div>
    );
  }
}
