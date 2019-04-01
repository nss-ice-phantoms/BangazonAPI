# Bangazon API

Bangazon is the virtual marketplace that will change lives and save babies.  This marketplace allows customers to buy and sell their products through a single page application web page and its data is tracked through a powerful, hand crafted and solely dedicated API. Whoa!

##API Documentation
Documentation can be found in the [Bangazon Wiki](https://github.com/nss-ice-phantoms/BangazonAPI)

##Software Requirements
Sql Server Manangment Studio Visual Studio Community 2017 Google Chrome

##Entity Relationship Diagram
Coming soon.

##Database Setup
Coming soon.

#HTTP Request Methods

#1. Customer

##GET

* select `GET` then paste `localhost:5000/customer` into the field and click send. The result should be an array of all the orders in the database.

*select `GET` then paste `localhost:5000/customer/?_include=payments` into the field and click send. The result should be an array of all the customers in the database with all of the payment types included in that customers as well.

*select `GET` then paste `localhost:5000/customer/?_include=products` into the field and click send. The result should be an array of all the customers in the database with all of the products types included in that customers as well.

*select `GET` then paste `localhost:5000/customer/?q=bo` into he field and click send. The result should be an array of all the customers in the database with first or last names that contains sat.

*select `GET` then paste `localhost:5000/customer/1` or any other number that showed up in the previous query as CustomerId and click send. The result should be only that object of the specified Customer

##POST

select `POST` then paste `localhost:5000/customer`into the field, then click Body underneath the field, then select raw, and then paste this snippet or make one similar

```
{
	"FirstName": "Testing",
	"LastName": "OneTwo"
   }
```
then click send. The result should be the new customer you made.

##PUT

select `PUT` then paste `localhost:5000/customer` or any other `Customer Id`, then click Body underneath the field, then select raw, and then paste this snippet or make one similar
```
{
	"FirstName": "Testing",
	"LastName": "NewLastName"
   }
```

You should get nothing back from this. When you run the `GET` query the Customer you specified in your `PUT` query should show the updated, edited information you gave it.





