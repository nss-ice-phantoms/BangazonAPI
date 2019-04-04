import React, { Component } from 'react';
import TrainingProgram from './TrainingProgram';

export default class TrainingProgramList  extends Component {
  render() {
    return (
      <React.Fragment>
      <section className="trainingPrograms">
      {
        this.props.trainingPrograms.map(trainingProgram => (
          <TrainingProgram key={trainingProgram.id} trainingProgram={[trainingProgram]}></TrainingProgram>
        ))
      }
      </section>
      </React.Fragment>
    );
  }
}