import React, { Component } from 'react';
import { WishlistItems } from './WishlistItems'
import { Loading } from './Loading'

export class Wishlist extends Component {

  constructor(props) {
    super(props);
    this.state = {
      wishlistData: [],
      loading: true
    }
  }

  componentDidMount() {
    this.populateWishlistData();
  }

  render() {
    const name = this.props.location.state.person.name;
    return (
      <div className="container mt-4" >
        <div className="row justify-content-center">
          <div className="col-11 col-md-10">
            <div className="card border-top-0 rounded-0">
              <div className="card-header">
                <h1 className="font-weight-light text-center">{name}'s Wishlist</h1>
              </div>

              <div className="card-body">

                {this.state.loading
                  ? <Loading />
                  : <WishlistItems wishlistData={this.state.wishlistData} personId={this.props.id} />}
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