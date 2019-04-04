import React, { Component } from 'react';
import Product from './Product';

export default class ProductList  extends Component {
  render() {
    return (
      <React.Fragment>
      <section className="products">
      {
        this.props.products.map(product => (
          <Product key={product.id} product={[product]}></Product>
        ))
      }
      </section>
      </React.Fragment>
    );
  }
}