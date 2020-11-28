import React, { Component } from "react";
import { navigate } from "@reach/router";

export class WhanauList extends Component {
  render() {
    const whanau = this.props.whanau;
    const myWhanau =
      whanau &&
      whanau.map((person) => {
        return (
          <li
            className="list-group-item justify-content-md-center"
            key={person.id}
          >
            <button
              className="btn btn-success btn-block"
              title={person.name}
              onClick={() => navigate(`/person/${person.id}/wishlist`)}
            >
              {person.name}
            </button>
          </li>
        );
      });

    return <ul className="list-group list-group-flush">{myWhanau}</ul>;
  }
}
