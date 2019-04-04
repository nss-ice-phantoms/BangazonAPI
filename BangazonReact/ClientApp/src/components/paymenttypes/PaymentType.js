import React, { Component } from 'react';

export default class PaymentType  extends Component {
  render() {
    return (
      this.props.paymentType.map(pt =>
        <div key={pt.id}>
          {'Name:  '}{pt.name}<br />
        </div>
      )
    );
  }
}