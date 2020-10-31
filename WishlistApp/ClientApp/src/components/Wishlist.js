import React, { Component } from 'react';
import { WishlistList } from './WishlistList'

export class Wishlist extends Component {

  constructor(props) {
    super(props);
    this.state = {
      wishlistData: []
    }
  }

  componentDidMount() {
    this.populateWishlistData();
  }

  render() {
    return (
      <div className="container mt-4" >
        <div className="row justify-content-center">
          <div className="col-11 col-md-6">
            <div className="card border-top-0 rounded-0">
              <div className="card-header">
                <h1 className="font-weight-light text-center">???'s Wishlist</h1>
              </div>

              <div className="card-body">
                <WishlistList wishlistData={this.state.wishlistData} />
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  async populateWishlistData() {
    const response = await fetch(`api/person/${this.props.id}/wishlist`);
    const data = await response.json();
    this.setState({ wishlistData: data, loading: false });
  }
}