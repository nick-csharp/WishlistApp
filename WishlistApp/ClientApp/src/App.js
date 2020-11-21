import React, { Component } from 'react';
import { Whanau } from './components/Whanau';
import { Wishlist } from './components/Wishlist';
import { navigate, Router } from "@reach/router";

import './custom.css'
import { Layout } from './components/Layout';
import { AuthTest } from './components/AuthTest';

export default class App extends Component {

  render () {
    return (
      <Layout>
        <Router>
          <Whanau path='/' />
          <AuthTest path='/authtest' />
          <Wishlist path='/person/:personId/wishlist' />
        </Router>
      </Layout>
    );
  }
}
