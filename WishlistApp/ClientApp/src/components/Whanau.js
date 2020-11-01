import React, { Component } from 'react';
import { WhanauList } from './WhanauList';
import { Loading } from './Loading';

export class Whanau extends Component {

  constructor(props) {
    super(props);
    this.state = { whanau: [], loading: true };
  }

  componentDidMount() {
    this.populateWhanauData();
  }

  render() {
    return (
      <div className="container mt-4" >
        <div className="row justify-content-center">
          <div className="col-md-8">
            <h1 className="font-weight-light text-center">Candy-Koedijk Whānau</h1>

            <div className="row justify-content-center">
              <div className="col-md-6">
                {this.state.loading
                  ? <Loading />
                  : <WhanauList whanau={this.state.whanau} />}
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  async populateWhanauData() {
    const response = await fetch('api/whanau/7c51567e-588b-4329-bfb9-361105853517');
    const data = await response.json();

    data.sort(function (a, b) {
      if (a.name < b.name) { return -1; }
      if (a.name > b.name) { return 1; }
      return 0;
    });

    this.setState({ whanau: data, loading: false });
  }
}