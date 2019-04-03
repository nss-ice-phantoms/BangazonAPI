import React, { Component } from 'react';

export default class Department  extends Component {
  render() {
    return (
      this.props.department.map(dept =>
        <div key={dept.id}>
          {'Name:  '}{dept.name}<br />
        </div>
      )
    );
  }
}