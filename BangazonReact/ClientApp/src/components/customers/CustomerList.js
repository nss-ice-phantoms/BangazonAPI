import React, { Component } from 'react';
import Customer from './Customer';

export default class CustomerList  extends Component {
  render() {
    return (
      <React.Fragment>
      <section className="customers">
      {
        this.props.customers.map(customer => (
          <Customer key={customer.id} customer={[customer]}></Customer>
        ))
      }
      </section>
      </React.Fragment>
    );
  }
}