import React, { Component } from 'react';

export default class Computer  extends Component {
  render() {
    return (
      this.props.computer.map(comp =>
        <div key={comp.id}>
          {'Name:  '}{comp.make}<br />
          {'Purchase Date:  '}{comp.purchaseDate}<br />
        </div>
      )
    );
  }
}