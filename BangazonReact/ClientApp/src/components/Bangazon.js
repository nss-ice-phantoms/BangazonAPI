import React, { Component } from 'react';
import NavBar from './nav/NavBar';
import ApplicationViews from './ApplicationViews';
// import bangazonData from '../modules/bangazonData';

export default class Bangazon extends Component {

  // constructor(props) {
  //   super(props);

  //   this.state = {
  //     jsonQuery: '',
  //     results: {
  //       studentsResults: [],
  //       instructorsResults: [],
  //       cohortsResults: [],
  //       exercisesResults: []
  //     }
  //   }

  //   this.handleInputChange = this.handleInputChange.bind(this);
  //   this.getInfo = this.getInfo.bind(this)
  // }

  // handleInputChange = (e) => {
  //   this.setState({
  //     jsonQuery: e.target.value
  //   }, () => {
  //     if (this.state.jsonQuery && this.state.jsonQuery.length >1) {
  //       this.getInfo();
  //     }
  //   })
  // }

  // getInfo () {
  //   let results = {
  //     studentsResults: [],
  //     instructorsResults: [],
  //     cohortsResults: [],
  //     exercisesResults: []
  //   }

  //   bangazonData.handleData({dataSet: 'students', fetchType: 'GET', embedItem: `?q=${this.state.jsonQuery}`})
  //   .then(students => {results.studentsResults = students})
  //   .then(() => bangazonData.handleData({dataSet: 'instructors', fetchType: 'GET', embedItem: `?q=${this.state.jsonQuery}`}))
  //   .then(instructors => {results.instructorsResults = instructors})
  //   .then(() => bangazonData.handleData({dataSet: 'cohorts', fetchType: 'GET', embedItem: `?q=${this.state.jsonQuery}`}))
  //   .then(cohorts => {results.cohortsResults = cohorts})
  //   .then(() => bangazonData.handleData({dataSet: 'exercises', fetchType: 'GET', embedItem: `?q=${this.state.jsonQuery}`}))
  //   .then(exercises => {results.exercisesResults = exercises; this.setState({results: results})})
  // }

  render() {
    return (
        <React.Fragment>
            <NavBar/>
            <ApplicationViews/>
        </React.Fragment>
    )
  }
}

/* <React.Fragment>
      <NavBar jsonQuery={this.state.jsonQuery} results={this.state.results} handleInputChange={this.handleInputChange}/>
      <ApplicationViews jsonQuery={this.state.jsonQuery} results={this.state.results} handleInputChange={this.handleInputChange}/>
  </React.Fragment> */