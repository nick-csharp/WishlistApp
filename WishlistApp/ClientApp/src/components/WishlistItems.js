import React, { Component } from 'react';
import { FaUndo } from "react-icons/fa"
import Emoji from './Emoji';

export class WishlistItems extends Component {

  constructor(props) {
    super(props);
    this.state = {
      isMe: false,
      wishlistData: props.wishlistData
    };

    this.claimOrUnclaimItem = this.claimOrUnclaimItem.bind(this);
    this.editItem = this.editItem.bind(this);
    this.deleteItem = this.deleteItem.bind(this);
    this.getButtons = this.getButtons.bind(this);
  }

  claimOrUnclaimItem(e, item, isClaim) {
    e.preventDefault();

    if (this.state.isMe) return;

    var confirmUnclaimText = "Are you sure you want to unclaim this? Someone else might claim it!";
    var requestUri = `api/person/${this.props.personId}/wishlist/${item.id}/`;
    var newIsClaimable = false, newIsClaimedByMe = false;

    if (isClaim && item.isClaimable) {
      // do claim
      requestUri += "claim";
      newIsClaimedByMe = true;
    }
    else if (!isClaim && item.isClaimedByMe && window.confirm(confirmUnclaimText)) {
      // do unclaim
      requestUri += "unclaim";
      newIsClaimable = true;
    }
    else {
      // invalid
      var errorMessage = "Invalid claim attempt."
        + ` isClaimable: ${item.isClaimable},`
        + ` isClaimedByMe: ${item.isClaimedByMe},`
        + ` isClaim: ${isClaim}`;

      console.log(errorMessage);
      return;
    }

    // submit the request
    fetch(requestUri, { method: 'PATCH' })
      .then(response => {
        if (!response.ok) {
          throw new Error("Network response was not ok.");
        }

        var index = this.state.wishlistData.findIndex((obj => obj.id == item.id));

        const newWishlistData = [...this.state.wishlistData];
        newWishlistData[index].isClaimable = newIsClaimable;
        newWishlistData[index].isClaimedByMe = newIsClaimedByMe;

        this.setState({ wishlistData: newWishlistData });
      })
      .catch(error => {
        console.error("There has been a problem with your fetch operation:", error);
      });
  };

  // TODO
  editItem(e, item) {
    e.preventDefault();
    alert("This function is not yet supported!");
  };
  
  deleteItem(e, item) {
    e.preventDefault();

    var shouldDelete = window.confirm("Are you sure you want to delete this? Someone might have got it for you already!");
    if (shouldDelete) {
      const deleteRequest = new Request(`api/person/${this.props.personId}/wishlist/${item.id}`, {
        method: 'DELETE'
      });

      fetch(deleteRequest)
        .then(response => {
          if (!response.ok) {
            throw new Error("Network response was not ok.");
          }

          const newWishlistData = this.state.wishlistData.filter(x => x.id != item.id);
          this.setState({ wishlistData: newWishlistData });
        })
        .catch(error => {
          console.error("There has been a problem with your fetch operation:", error);
        });
    }
  };

  getButtons(item) {
    if (this.state.isMe) {
      return (
        <React.Fragment>
          <button
            type="button"
            className="btn btn-sm btn-outline-secondary emoji-button"
            title="Edit item"
            onClick={(e) => this.editItem(e, item)}
          >
            <Emoji symbol="✏️" label="present" />
          </button>

          <button
            type="button"
            className="btn btn-sm btn-outline-secondary emoji-button"
            title="Delete item"
            onClick={(e) => this.deleteItem(e, item)}
          >
            <Emoji symbol="🗑" label="bin" />
          </button>
        </React.Fragment>
      );
    }
    else if (item.isClaimable) {
      return (
        <button
          type="button"
          className="btn btn-sm btn-outline-secondary emoji-button"
          style={{ width: 35 }}
          onClick={(e) => this.claimOrUnclaimItem(e, item, true)}
          title="Claim"
        >
          <Emoji symbol="🎁" label="present" />
        </button>
      );
    }
    else if (item.isClaimedByMe) {
      return (
        <button
          type="button"
          className="btn btn-sm btn-outline-secondary emoji-button"
          style={{ width: 35 }}
          onClick={(e) => this.claimOrUnclaimItem(e, item, false)}
          title="Unclaim"
        >
          <Emoji symbol="✔️" label="tick" />
        </button>
      );
    }
    else if (!item.isClaimable) {
      return (
        <button
          type="button"
          className="btn btn-sm btn-outline-secondary"
          title="Claimed by someone else!"
          style={{ border:"none" }}
          disabled
        >
          <Emoji symbol="🔒" label="lock" />
        </button>
      );
    }
    else {
      return (null);
    }
  };

  render() {
    const wishlist = this.state.wishlistData;

    if (!wishlist) {
      return (null);
    }

    const getClaimStyle = (item) => {
      if (item.isClaimedByMe) {
        return "claimed-by-me";
      }
      else if (!this.state.isMe && !item.isClaimable) {
        return "claimed-by-other";
      }
      else {
        return "unclaimed";
      }
    };

    const myWishlist = wishlist.map((item) => {
      return (
        <li className={"list-group-item " + getClaimStyle(item)} key={item.id}>
          <div className="d-flex">
            <section className="align-self-center item-text">
              {item.description}
            </section>
            <div className="ml-auto">
              <div className="btn-group"
                role="group"
                aria-label="Wishlist Options"
              >
                {this.getButtons(item)}
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