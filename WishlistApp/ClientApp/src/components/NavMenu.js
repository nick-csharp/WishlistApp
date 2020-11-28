import React, { Component } from "react";
import { Link } from "@reach/router";
import { MsalAuthContext } from "./msal/MsalAuthProvider";

export default class NavMenu extends Component {
  static displayName = NavMenu.name;
  static contextType = MsalAuthContext;

  getEmoji() {
    const useOtherEmoji = Math.random() > 0.8;

    if (useOtherEmoji) {
      const emojis = ["🎅", "🎁", "🧦", "🤶", "📯", "🌟", "🌠", "👀", "🏖️"];
      var index = Math.floor(Math.random() * emojis.length);
      return emojis[index];
    } else {
      return "🎄";
    }
  }

  render() {
    const isAuthenticated = this.context.isAuthenticated;
    const account = this.context.account;
    const logout = this.context.logout;
    const welcomeMessage = "Welcome " + account.name + "!";

    return (
      <nav className="navbar navbar-expand navbar-dark color-nav">
        <div className="navbar-brand">{this.getEmoji()} Wishlists</div>
        {isAuthenticated && (
          <React.Fragment>
            <div className="navbar-brand abs">
              <span>{welcomeMessage}</span>
            </div>
            <div className="navbar-nav ml-auto">
              <Link className="nav-item nav-link" to="/whanau">
                Whānau
              </Link>
              <Link
                className="nav-item nav-link"
                to="/logout"
                onClick={(e) => logout(e)}
              >
                Log out
              </Link>
            </div>
          </React.Fragment>
        )}
      </nav>
    );
  }
}
