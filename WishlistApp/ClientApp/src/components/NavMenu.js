import React, { Component } from 'react';
import { FaUsers } from "react-icons/fa";
import { Link } from "@reach/router";

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor (props) {
    super(props);
  }

  getEmoji() {
    const useOtherEmoji = Math.random() > 0.8;

    if (useOtherEmoji) {
      const emojis = ["🎅", "🎁", "🧦", "🤶", "📯", "🌟", "🌠", "👀", "🏖️"];
      var index = Math.floor(Math.random() * emojis.length);
      return emojis[index];
    } else {
      return ("🎄");
    }   
  }

  render() {
    const user = null;
    const logOutUser = null;

    return (
      <nav className="navbar navbar-expand navbar-dark color-nav">
        <div className="container-fluid">
          <div className="navbar-brand">
            {this.getEmoji()} Wishlists
          </div>
          <div className="navbar-nav ml-auto">
            {!user && (
              <Link className="nav-item nav-link" to="/">
                Whānau
              </Link>
            )}
            {/*!user && (
              <Link className="nav-item nav-link" to="/authtest">
                Auth test
              </Link>
            )*/}
            {user && (
              <Link
                className="nav-item nav-link"
                to="/login"
                onClick={(e) => logOutUser(e)}
              >
                log out
              </Link>
            )}
          </div>
        </div>
      </nav>
    );
  }
}


// <nav className="site-nav family-sans navbar  bg-primary navbar-dark higher"> 