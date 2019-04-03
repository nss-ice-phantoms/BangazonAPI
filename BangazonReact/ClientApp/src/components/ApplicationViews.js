import { Route } from 'react-router-dom';
import React, { Component } from "react";
import banazondData from '../modules/bangazonData'
import DepartmentList from './departments/DepartmentList';
// import SearchResults from './nav/SearchResults';
// import ExerciseList from './exercises/ExerciseList';
// import InstructorList from './instructors/InstructorList';
// import CohortList from './cohorts/CohortList';

export default class ApplicationViews extends Component {
  constructor() {
    super();
    this.state = {

      departments: [],
      // instructors: [],
      // cohorts: [],
      // exercises: []
    }
  }

  populateAppState () {
    banazondData.handleData({dataSet: 'departments', fetchType: 'GET', embedItem: ""})
      .then(departments => {this.setState({departments: departments}, ()=>null)})
      // .then(() => banazondData.handleData({dataSet: 'instructors', fetchType: 'GET', embedItem: "?_expand=employee"}))
      // .then(instructors => {this.setState({instructors: instructors}, ()=>null)})
      // .then(() => banazondData.handleData({dataSet: 'cohorts', fetchType: 'GET', embedItem: ""}))
      // .then(cohorts => {this.setState({cohorts: cohorts}, ()=>null)})
      // .then(() => banazondData.handleData({dataSet: 'exercises', fetchType: 'GET', embedItem: ""}))
      // .then(exercises => {this.setState({exercises: exercises}, ()=>null)})
  }

  componentDidMount () {
    this.populateAppState();
  }

  render() {
    return (
      <React.Fragment>
        <Route exact path="/" render={(props) => {
          return <DepartmentList {...props} departments={this.state.departments} />}} />
        {/* <Route exact path="/exercises" render={(props) => {
          return <ExerciseList exercises={this.state.exercises}  />}} />
        <Route exact path="/instructors" render={(props) => {
          return <InstructorList {...props} instructors={this.state.instructors}/>}} />
        <Route exact path="/cohorts" render={(props) => {
          return <CohortList {...props} cohorts={this.state.cohorts}/>}} />
        <Route exact path="/searchresults" render={(props) => {
          return <SearchResults jsonQuery={this.props.jsonQuery} results={this.props.results} handleInputChange={this.props.handleInputChange}/>}} /> */}
      </React.Fragment>
    )
  }
}