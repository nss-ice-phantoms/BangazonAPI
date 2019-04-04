import React, { Component } from 'react';
import Department from './Computer';

export default class ComputerList   extends Component {
  render() {
    return (
      <React.Fragment>
      <section className="computers">
      {
        this.props.computers.map(computer => (
          <Department key={computer.id} computer={[computer]}></Department>
        ))
      }
      </section>
      </React.Fragment>
    );
  }
}