import {
  b2cPolicies,
  msalConfig,
  loginRequest,
  tokenRequest,
} from "./MsalConfig";
import {
  PublicClientApplication,
  InteractionRequiredAuthError,
} from "@azure/msal-browser";
import React, { Component } from "react";

export const MsalAuthContext = React.createContext();

export function withAuth(HocComponent) {
  return class extends Component {
    constructor(props) {
      super(props);

      this.state = {
        account: {},
        error: null,
        isAuthenticated: false,
      };

      this.login = this.login.bind(this);
      this.logout = this.logout.bind(this);
      this.getAccessToken = this.getAccessToken.bind(this);
      this.handleResponse = this.handleResponse.bind(this);
      this.handleError = this.handleError.bind(this);
      this.appendAccessToken = this.appendAccessToken.bind(this);

      this.msalAuth = new PublicClientApplication(msalConfig);
      this.msalAuth
        .handleRedirectPromise()
        .then(this.handleResponse)
        .catch(this.handleError);
    }

    handleResponse(resp) {
      if (resp !== null) {
        this.setState({ isAuthenticated: true, account: resp.account });
      } else {
        const accounts = this.msalAuth.getAllAccounts();
        if (accounts && accounts.length > 0) {
          this.setState({ isAuthenticated: true, account: accounts[0] });
        } else {
          this.setState({ isAuthenticated: false, account: {} });
        }
      }
    }

    async handleError(error) {
      // Check for forgot password error
      // Learn more about AAD error codes at https://docs.microsoft.com/en-us/azure/active-directory/develop/reference-aadsts-error-codes
      if (
        error &&
        error.errorMessage &&
        error.errorMessage.indexOf("AADB2C90118") > -1
      ) {
        try {
          return await this.msalAuth.loginRedirect(
            b2cPolicies.authorities.forgotPassword
          );
        } catch (err) {
          console.log(err);
        }
      }
    }

    async login() {
      return await this.msalAuth.loginRedirect(loginRequest);
    }

    async logout() {
      const logoutRequest = {
        account: this.msalAuth.getAccountByHomeId(this.state.account.accountId),
      };
      return await this.msalAuth.logout(logoutRequest);
    }

    async appendAccessToken(options) {
      const token = await this.getAccessToken();
      const bearer = `Bearer ${token}`;
      if (!options.headers) {
        options.headers = new Headers();
      }
      options.headers.append("Authorization", bearer);
    }

    async getAccessToken() {
      let request = tokenRequest;
      request.account = this.state.account;
      let token = await this.msalAuth
        .acquireTokenSilent(request)
        .catch(async (error) => {
          console.log("silent token acquisition fails.");
          if (error instanceof InteractionRequiredAuthError) {
            // fallback to interaction when silent call fails
            console.log("acquiring token using redirect");
            this.msalAuth.acquireTokenRedirect(request);
          } else {
            console.error(error);
          }
        });

      if (!token || !token.accessToken) {
        console.log("token was missing - acquiring token using redirect");
        this.msalAuth.acquireTokenRedirect(request);
      }

      return token.accessToken;
    }

    render() {
      return (
        <MsalAuthContext.Provider
          value={{
            isAuthenticated: this.state.isAuthenticated,
            account: this.state.account,
            loading: this.state.loading,
            login: this.login,
            appendAccessToken: this.appendAccessToken,
            logout: this.logout,
          }}
        >
          <HocComponent />
        </MsalAuthContext.Provider>
      );
    }

    // normalizeError(error) {
    //   var normalizedError = {};
    //   if (typeof error === "string") {
    //     var errParts = error.split("|");
    //     normalizedError =
    //       errParts.length > 1
    //         ? { message: errParts[1], debug: errParts[0] }
    //         : { message: error };
    //   } else {
    //     normalizedError = {
    //       message: error.message,
    //       debug: JSON.stringify(error),
    //     };
    //   }
    //   return normalizedError;
    // }
  };
}
