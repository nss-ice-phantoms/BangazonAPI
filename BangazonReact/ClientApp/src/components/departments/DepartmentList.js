import React, { Component } from 'react';
import Department from './Department';

export default class DepartmentList  extends Component {
  render() {
    return (
      <React.Fragment>
      <section className="departments">
      {
        this.props.departments.map(department => (
          <Department key={department.id} department={[department]}></Department>
        ))
      }
      </section>
      </React.Fragment>
    );
  }
}