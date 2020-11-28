import React, { Component } from "react";
import { WhanauList } from "./WhanauList";
import Loading from "./Loading";
import { MsalAuthContext } from "./msal/MsalAuthProvider";

export default class Whanau extends Component {
  static contextType = MsalAuthContext;
  constructor(props) {
    super(props);
    this.state = {
      whanauName: "",
      whanauData: [],
      loading: true,
    };
  }

  componentDidMount() {
    this.populateWhanauData();
  }

  render() {
    return (
      <div className="container mt-4">
        <div className="row justify-content-center">
          <div className="col-md-8">
            <div className="card">
              {this.state.loading ? (
                <Loading />
              ) : (
                <React.Fragment>
                  <div className="card-header">
                    <h1 className="font-weight-light text-center">
                      {this.state.whanauName} Whānau
                    </h1>
                  </div>

                  <div className="card-body" style={{ padding: "0px" }}>
                    <WhanauList
                      whanau={this.state.whanauData}
                      userId={this.state.userId}
                    />
                  </div>
                </React.Fragment>
              )}
            </div>
          </div>
        </div>
      </div>
    );
  }

  async populateWhanauData() {
    const options = {
      method: "GET",
    };
    await this.context.appendAccessToken(options);
    const response = await fetch("api/whanau", options);
    const whanau = await response.json();

    whanau.people.sort(function (a, b) {
      if (a.name < b.name) {
        return -1;
      }
      if (a.name > b.name) {
        return 1;
      }
      return 0;
    });

    this.setState({
      whanauName: whanau.name,
      whanauData: whanau.people,
      loading: false,
    });
  }
}
