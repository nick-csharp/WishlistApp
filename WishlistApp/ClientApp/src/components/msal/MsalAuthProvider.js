import {
  msalConfig,
  loginRequest,
  tokenRequest,
} from "./MsalConfig";
import {
  PublicClientApplication,
  InteractionRequiredAuthError,
} from "@azure/msal-browser";
import React, { Component } from "react";

export function withAuth(HocComponent) {
  return class extends Component {
    constructor(props) {
      super(props);

      this.state = {
        accountId: "",
        error: null,
        isAuthenticated: false,
        user: {},
      };

      this.handleResponse = this.handleResponse.bind(this);
      this.getAccessToken = this.getAccessToken.bind(this);
      this.signIn = this.signIn.bind(this);
      this.signOut = this.signOut.bind(this);

      this.msalAuth = new PublicClientApplication(msalConfig);
    }

    async componentDidMount() {
      this.msalAuth
        .handleRedirectPromise()
        .then(this.handleResponse)
        .catch((error) => {
          console.error(error);
          // Check for forgot password error
          //// Learn more about AAD error codes at https://docs.microsoft.com/en-us/azure/active-directory/develop/reference-aadsts-error-codes
          //if (error.errorMessage.indexOf("AADB2C90118") > -1) {
          //  try {
          //    msalAuth.loginRedirect(b2cPolicies.authorities.forgotPassword);
          //  } catch (err) {
          //    console.log(err);
          //  }
          //}
        });
    }

    handleResponse(resp) {
      /**
       * See here for more information on account retrieval:
       * https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-common/docs/Accounts.md
       */
      debugger;
      if (resp !== null) {
        this.setState({ accountId: resp.account.homeAccountId });
      } else {
        // need to call getAccount here?
        const currentAccounts = this.msalAuth.getAllAccounts();
        if (!currentAccounts || currentAccounts.length < 1) {
          this.signIn();
        } else if (currentAccounts.length > 1) {
          // Add choose account code here
          console.error(
            "Multiple accounts detected. Unable to handle this scenario"
          );
        } else if (currentAccounts.length === 1) {
          this.setState({ accountId: currentAccounts[0].homeAccountId });
        }
      }
    }

    async getAccessToken() {
      debugger;
      tokenRequest.account = this.msalAuth.getAccountByHomeId(this.state.accountId);
      return await this.msalAuth
        .acquireTokenSilent(tokenRequest)
        .catch(async (error) => {
          console.log("silent token acquisition fails.");
          if (error instanceof InteractionRequiredAuthError) {
            // fallback to interaction when silent call fails
            console.log("acquiring token using redirect");
            this.msalAuth.acquireTokenRedirect(tokenRequest);
          } else {
            console.error(error);
          }
        });
    }

    signIn() {
      return this.msalAuth.loginRedirect(loginRequest);
    }

    signOut() {
      const logoutRequest = {
        account: this.msalAuth.getAccountByHomeId(this.state.accountId),
      };
      this.msalAuth.logout(logoutRequest);
    }

    render() {
      return (
        <HocComponent
          error={this.state.error}
          isAuthenticated={this.state.isAuthenticated}
          user={this.state.user}
          onSignIn={() => this.signIn()}
          onSignOut={() => this.signOut()}
          getAccessToken={() => this.getAccessToken()}
          {...this.props}
          {...this.state}
        />
      );
    }
  };
}
