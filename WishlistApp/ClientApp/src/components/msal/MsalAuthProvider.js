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
        msalClient: {},
        isLoading: true,
        isAuthenticated: false,
        account: {},
        error: null,
      };

      this.login = this.login.bind(this);
      this.logout = this.logout.bind(this);
      this.getAccessToken = this.getAccessToken.bind(this);
      this.handleError = this.handleError.bind(this);
      this.appendAccessToken = this.appendAccessToken.bind(this);
    }

    async componentDidMount() {
      const msalClientOnMount = new PublicClientApplication(msalConfig);
      this.setState({ msalClient: msalClientOnMount });

      var response = await msalClientOnMount
        .handleRedirectPromise()
        .catch(this.handleError);

      if (
        response &&
        response.idTokenClaims &&
        response.idTokenClaims.tfp === b2cPolicies.names.forgotPassword
      ) {
        window.alert(
          "Password has been reset successfully. \nPlease sign-in with your new password."
        );
        await msalClientOnMount.logout();
      }

      const accounts = msalClientOnMount.getAllAccounts();
      if (accounts && accounts.length > 0) {
        this.setState({ isAuthenticated: true, account: accounts[0] });
      } else {
        this.setState({ isAuthenticated: false });
      }

      this.setState({ isLoading: false });
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
          return await this.state.msalClient.loginRedirect(
            b2cPolicies.authorities.forgotPassword
          );
        } catch (err) {
          console.log(err);
        }
      }
    }

    async login() {
      return await this.state.msalClient.loginRedirect(loginRequest);
    }

    async logout() {
      const logoutRequest = {
        account: this.state.msalClient.getAccountByHomeId(
          this.state.account.accountId
        ),
      };
      return await this.state.msalClient.logout(logoutRequest);
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
      
      let token = await this.state.msalClient
        .acquireTokenSilent(request)
        .catch(async (error) => {
          console.log("silent token acquisition fails.");
          if (error instanceof InteractionRequiredAuthError) {
            // fallback to interaction when silent call fails
            console.log("acquiring token using redirect");
            this.state.msalClient.acquireTokenRedirect(request);
          } else {
            console.error(error);
          }
        });

      if (!token || !token.accessToken) {
        console.log("token was missing - acquiring token using redirect");
        this.state.msalClient.acquireTokenRedirect(request);
      }

      return token.accessToken;
    }

    render() {
      return (
        <MsalAuthContext.Provider
          value={{
            isLoading: this.state.isLoading,
            isAuthenticated: this.state.isAuthenticated,
            account: this.state.account,
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
