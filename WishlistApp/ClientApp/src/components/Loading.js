import React, { Component } from "react";

export default class Loading extends Component {
  render() {
    return (
      <div className="d-flex justify-content-center">
        <div className="spinner-border text-success" role="status">
          <span className="sr-only">Loading...</span>
        </div>
      </div>
    );
  }
}
