import React, { Component } from "react";
import WishlistItems from "./WishlistItems";
import Loading from "./Loading";
import Emoji from "./Emoji";
import { MsalAuthContext } from "./msal/MsalAuthProvider";

export default class Wishlist extends Component {
  static contextType = MsalAuthContext;
  constructor(props) {
    super(props);
    this.state = {
      wishlistData: [],
      newItemDescription: "",
      isLoading: true,
      isMyWishlist: false,
      wishlistOwnerName: "",
    };

    this.handleChange = this.handleChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
    this.populateWishlistData = this.populateWishlistData.bind(this);
  }

  componentDidMount() {
    this.populateWishlistData();
  }

  async populateWishlistData() {
    var options = {};
    await this.context.appendAccessToken(options);

    const response = await fetch(
      `api/person/${this.props.personId}/wishlist`,
      options
    );
    const data = await response.json();
    this.setState({
      wishlistOwnerName: data.wishlistOwnerName,
      isMyWishlist: data.isMyWishlist,
      wishlistData: data.wishlistItems,
      isLoading: false,
    });
  }

  handleChange(e) {
    e.preventDefault();

    const itemName = e.target.name;
    const itemValue = e.target.value;

    this.setState({ [itemName]: itemValue });
  }

  async handleSubmit(e) {
    e.preventDefault();

    if (this.state.newItemDescription === "") {
      return;
    }

    var personId = this.props.personId;

    var options = {
      method: "POST",
      headers: new Headers({ "Content-Type": "application/json" }),
      body: JSON.stringify({
        userId: personId,
        description: this.state.newItemDescription,
      }),
    };

    await this.context.appendAccessToken(options);
    fetch(`api/person/${personId}/wishlist`, options)
      .then((response) => {
        if (!response.ok) {
          throw new Error("Network response was not ok.");
        }

        this.setState({ newItemDescription: "", isLoading: true });
        return this.populateWishlistData();
      })
      .catch((error) => {
        console.error(
          "There has been a problem with your fetch operation:",
          error
        );
      });
  }

  render() {
    const name = this.state.wishlistOwnerName;
    var body;

    if (this.state.isLoading) {
      body = () => {
        return <Loading />;
      };
    } else {
      body = () => {
        return (
          <div className="card">
            <div className="card-header">
              <h1 className="font-weight-light text-center">
                {name}'s Wishlist
              </h1>
            </div>

            {this.state.isMyWishlist ? (
              <div className="card-body">
                <form
                  className="formgroup"
                  autoComplete="off"
                  onSubmit={this.handleSubmit}
                >
                  <div className="input-group">
                    <input
                      type="text"
                      className="form-control"
                      name="newItemDescription"
                      placeholder="Add a wishlist item"
                      aria-describedby="buttonAdd"
                      value={this.state.newItemDescription}
                      onChange={this.handleChange}
                    />
                    <div className="input-group-append">
                      <button
                        type="submit"
                        className="btn btn-sm btn-outline-secondary emoji-button"
                        title="Add"
                        id="buttonAdd"
                      >
                        <Emoji symbol="➕" label="plus" />
                      </button>
                    </div>
                  </div>
                </form>
              </div>
            ) : null}

            <div className="card-body">
              <WishlistItems
                wishlistData={this.state.wishlistData}
                personId={this.props.personId}
                isMyWishlist={this.state.isMyWishlist}
              />
            </div>
          </div>
        );
      };
    }

    return (
      <div className="container mt-4">
        <div className="row justify-content-center">
          <div className="col-md-10">{body()}</div>
        </div>
      </div>
    );
  }
}
