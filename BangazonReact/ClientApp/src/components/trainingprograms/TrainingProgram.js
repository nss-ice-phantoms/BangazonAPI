import React, { Component } from 'react';

export default class TrainingProgram  extends Component {
  render() {
    return (
      this.props.trainingProgram.map(tp =>
        <div key={tp.id}>
          {'Name:  '}{tp.name}<br />
          {'Start Date:  '}{tp.startDate}<br />
          {'End Date:  '}{tp.endDate}<br />
        </div>
      )
    );
  }
}