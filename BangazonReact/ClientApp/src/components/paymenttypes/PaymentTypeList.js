import React, { Component } from 'react';
import PaymentType from './PaymentType';

export default class PaymentTypeList  extends Component {
  render() {
    return (
      <React.Fragment>
      <section className="paymentTypes">
      {
        this.props.paymentTypes.map(paymentType => (
          <PaymentType key={paymentType.id} paymentType={[paymentType]}></PaymentType>
        ))
      }
      </section>
      </React.Fragment>
    );
  }
}