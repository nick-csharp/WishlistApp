import React, { Component } from 'react';
import { WhanauList } from './WhanauList';
import { Loading } from './Loading';
import { AuthContext } from './AuthContext';

class Whanau extends Component {

  constructor(props) {
    super(props);
    this.state = {
      whanauName: "",
      userId: "",
      whanauData: [],
      loading: true
    };
  }

  componentDidMount() {
    this.populateWhanauData();
  }

  render() {
    return (
      <div className="container mt-4" >
        <div className="row justify-content-center">
          <div className="col-md-6">
            <div className="card">

              {this.state.loading
                ? <Loading />
                : <React.Fragment> 
                    <div className="card-header">
                    <h1 className="font-weight-light text-center">{this.state.whanauName} Whānau</h1>
                    </div>

                    <div className="card-body" style={{padding: "0px"}}>
                    <WhanauList whanau={this.state.whanauData} userId={this.state.userId} />
                    </div>
                  </React.Fragment>
                }
            </div>
          </div>
        </div>
      </div>
    );
  }

  async populateWhanauData() {
    const defaultWhanauResponse = await fetch("api/whanau");
    const defaultWhanau = await defaultWhanauResponse.json();

    const response = await fetch('api/whanau/' + defaultWhanau.defaultWhanauId);
    const data = await response.json();

    data.sort(function (a, b) {
      if (a.name < b.name) { return -1; }
      if (a.name > b.name) { return 1; }
      return 0;
    });

    this.setState({
      whanauName: defaultWhanau.defaultWhanauName,
      userId: defaultWhanau.defaultUserId,
      whanauData: data,
      loading: false
    });
  }
}
Whanau.contextType = AuthContext;

export default Whanau;