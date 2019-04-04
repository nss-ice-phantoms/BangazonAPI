import React, { Component } from 'react';

export default class Product  extends Component {
  render() {
    return (
      this.props.product.map(prod =>
        <div key={prod.id}>
          {'Name:  '}{prod.title}<br />
          {'Description:  '}{prod.description}<br />
        </div>
      )
    );
  }
}