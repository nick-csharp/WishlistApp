import React, { Component } from 'react';
import { GoTrashcan } from "react-icons/go"
import Emoji from './Emoji';

export class WishlistList extends Component {

  constructor(props) {
    super(props);
    this.state = { isMe: false };
  }

  render() {
    const wishlist = this.props.wishlistData;
    const myWishlist = wishlist && wishlist.map((item) => {
      return (
        <li className="list-group-item" key={item.id}>
          <div className="d-flex">
            <section className="align-self-center">
              {item.description}
            </section>
            <div className="ml-auto">
              <div className="btn-group"
                role="group"
                aria-label="Wishlist Options"
              >

                {this.state.isMe ?
                  <button
                    type="button"
                    className="btn btn-sm btn-outline-secondary"
                    title="Edit item">
                    <Emoji symbol="✏️" label="pencil" />
                  </button>
                  : null}

                {!this.state.isMe && item.isClaimable}

                <button
                  type="button"
                  className="btn btn-sm btn-outline-secondary"
                  title="Claim item">
                  <Emoji symbol="🎁" label="gift"/>
                </button>


                {this.state.isMe ?
                  <button
                    type="button"
                    className="btn btn-sm btn-outline-secondary"
                    title="Delete item"
                  >
                    <Emoji symbol="🗑" label="rubbish bin" />
                  </button>
                  : null}
              </div>
            </div>
          </div>
        </li>
      )
    });

    return (
      <ul className="list-group ">
        {myWishlist}
      </ul>
    );
  }
}