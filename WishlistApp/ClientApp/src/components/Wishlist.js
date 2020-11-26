import React, { Component } from "react";
import WishlistItems from "./WishlistItems";
import Loading from "./Loading";
import Emoji from "./Emoji";
import { MsalAuthContext } from "./msal/MsalAuthProvider";

export class Wishlist extends Component {
  static contextType = MsalAuthContext;
  constructor(props) {
    super(props);
    this.state = {
      wishlistData: [],
      newItemDescription: "",
      isLoading: true,
      isMe: false,
    };

    debugger;

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
      `api/person/${this.props.personId}/wishlist?requestingUserId=${this.props.location.state.userId}`,
      options
    );
    const data = await response.json();
    this.setState({
      isMe: data.isMyWishlist,
      wishlistData: data.wishlistItems,
      isLoading: false
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
      headers: new Headers({"Content-Type": "application/json"}),
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
    const name = this.props.location.state.person.name;
    return (
      <div className="container mt-4">
        <div className="row justify-content-center">
          <div className="">
            <div className="card">
              <div className="card-header">
                <h1 className="font-weight-light text-center">
                  {name}'s Wishlist
                </h1>
              </div>

              {this.state.isMe ? (
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
                {this.state.isLoading ? (
                  <Loading />
                ) : (
                  <WishlistItems
                    wishlistData={this.state.wishlistData}
                    personId={this.props.personId}
                    isMe={this.state.isMe}
                  />
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }
}
