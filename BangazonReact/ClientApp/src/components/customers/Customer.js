import React, { Component } from 'react';

export default class Customer  extends Component {
  render() {
    return (
      this.props.customer.map(cust =>
        <div key={cust.id}>
          {'Name:  '}{cust.firstName} {cust.lastName}<br />
        </div>
      )
    );
  }
}