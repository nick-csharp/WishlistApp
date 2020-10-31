import React, { Component } from 'react';
import { FaUsers } from "react-icons/fa";
import { Link } from "@reach/router";

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor (props) {
    super(props);
  }

  render() {
    const user = null;
    const logOutUser = null;

    return (
      <nav className="navbar navbar-expand navbar-dark color-nav">
        <div className="container-fluid">
          <Link to="/" className="navbar-brand">
            <FaUsers className="mr-1" /> Meeting Log
          </Link>
          <div className="navbar-nav ml-auto">
            {!user && (
              <Link className="nav-item nav-link" to="/whanau">
                Whānau
              </Link>
            )}
            {!user && (
              <Link className="nav-item nav-link" to="/login">
                Log in
              </Link>
            )}
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