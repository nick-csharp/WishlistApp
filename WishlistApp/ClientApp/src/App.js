import React, { Component } from 'react';
import { Home } from './components/Home';
import { Whanau } from './components/Whanau';
import { Wishlist } from './components/Wishlist';
import { navigate, Router } from "@reach/router";

import './custom.css'
import { Layout } from './components/Layout';

export default class App extends Component {

  render () {
    return (
      <Layout>
        <Router>

          <Home path='/' />
          <Whanau path='/whanau' />
          <Wishlist path='/person/:id/wishlist' />
        </Router>
      </Layout>
    );
  }
}
