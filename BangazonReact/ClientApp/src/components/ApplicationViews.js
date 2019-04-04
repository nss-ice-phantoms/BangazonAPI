import { Route } from 'react-router-dom';
import React, { Component } from "react";
import banazondData from '../modules/bangazonData'
import DepartmentList from './departments/DepartmentList';
import ComputerList from './computers/ComputerList';
import CustomerList from './customers/CustomerList';
import PaymentTypesList from './paymentTypes/PaymentTypeList';
import ProductList from './products/ProductList';
import TrainingProgramList from './trainingPrograms/TrainingProgramList';
// import SearchResults from './nav/SearchResults';

export default class ApplicationViews extends Component {
  constructor() {
    super();
    this.state = {

      departments: [],
      computers: [],
      customers: [],
      paymentTypes: [],
      products: [],
      trainingPrograms: []
    }
  }

  populateAppState () {
    banazondData.handleData({dataSet: 'departments', fetchType: 'GET', embedItem: ""})
      .then(departments => {this.setState({departments: departments}, ()=>null)})
      .then(() => banazondData.handleData({dataSet: 'computers', fetchType: 'GET', embedItem: ""}))
      .then(computers => {this.setState({computers: computers}, ()=>null)})
      .then(() => banazondData.handleData({dataSet: 'customers', fetchType: 'GET', embedItem: ""}))
      .then(customers => {this.setState({customers: customers}, ()=>null)})
      .then(() => banazondData.handleData({dataSet: 'paymentTypes', fetchType: 'GET', embedItem: ""}))
      .then(paymentTypes => {this.setState({paymentTypes: paymentTypes}, ()=>null)})
      .then(() => banazondData.handleData({dataSet: 'products', fetchType: 'GET', embedItem: ""}))
      .then(products => {this.setState({products: products}, ()=>null)})
      .then(() => banazondData.handleData({dataSet: 'trainingPrograms', fetchType: 'GET', embedItem: ""}))
      .then(trainingPrograms => {this.setState({trainingPrograms: trainingPrograms}, ()=>null)})
  }

  componentDidMount () {
    this.populateAppState();
  }

  render() {
    return (
      <React.Fragment>
        <Route exact path="/" render={(props) => {
          return <DepartmentList {...props} departments={this.state.departments} />}} />
        <Route exact path="/computers" render={(props) => {
          return <ComputerList {...props} computers={this.state.computers}  />}} />
        <Route exact path="/customers" render={(props) => {
          return <CustomerList {...props} customers={this.state.customers}/>}} />
        <Route exact path="/paymentTypes" render={(props) => {
          return <PaymentTypesList {...props} paymentTypes={this.state.paymentTypes}/>}} />
        <Route exact path="/products" render={(props) => {
          return <ProductList {...props} products={this.state.products}/>}} />
        <Route exact path="/trainingPrograms" render={(props) => {
          return <TrainingProgramList {...props} trainingPrograms={this.state.trainingPrograms}/>}} />
         {/* <Route exact path="/searchresults" render={(props) => {
          return <SearchResults jsonQuery={this.props.jsonQuery} results={this.props.results} handleInputChange={this.props.handleInputChange}/>}} /> */}
      </React.Fragment>
    )
  }
}