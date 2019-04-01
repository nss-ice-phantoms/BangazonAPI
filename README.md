# Bangazon API

Bangazon is the virtual marketplace that will change lives and save babies.  This marketplace allows customers to buy and sell their products through a single page application web page and its data is tracked through a powerful, hand crafted and solely dedicated API. Whoa!

## API Documentation
Documentation can be found in the [Bangazon Wiki](https://github.com/nss-ice-phantoms/BangazonAPI)

## Software Requirements
Sql Server Manangment Studio Visual Studio Community 2017 Google Chrome

## Entity Relationship Diagram
Coming soon.

## Database Setup
Coming soon.

# HTTP Request Methods

# 1. Customer

## GET

* select `GET` then paste `localhost:5000/customers` into the field and click send. The result should be an array of all the orders in the database.

* select `GET` then paste `localhost:5000/customers/?_include=payments` into the field and click send. The result should be an array of all the customers in the database with all of the payment types included in that customers as well.

* select `GET` then paste `localhost:5000/customers/?_include=products` into the field and click send. The result should be an array of all the customers in the database with all of the products types included in that customers as well.

* select `GET` then paste `localhost:5000/customers/?q=bo` into he field and click send. The result should be an array of all the customers in the database with first or last names that contains sat.

* select `GET` then paste `localhost:5000/customers/1` or any other number that showed up in the previous query as CustomerId and click send. The result should be only that object of the specified Customer

## POST

select `POST` then paste `localhost:5000/customers`into the field, then click Body underneath the field, then select raw, and then paste this snippet or make one similar

```
{
	"FirstName": "Testing",
	"LastName": "OneTwo"
   }
```
then click send. The result should be the new customer you made.

## PUT

select `PUT` then paste `localhost:5000/customers` or any other `Customer Id`, then click Body underneath the field, then select raw, and then paste this snippet or make one similar
```
{
	"FirstName": "Testing",
	"LastName": "NewLastName"
   }
```

You should get nothing back from this. When you run the `GET` query the Customer you specified in your `PUT` query should show the updated, edited information you gave it.

# 2. Payment Types
Use the command `dotnet run` to start the program, BangazonAPI. Once the program is running, open up the Postman desktop app and run the following commands for each request method:

`GET`
To GET all product types, select GET in Postman then paste `localhost:5000/PaymentTypes` into the field and click send. The result should be an array of all the payment type in the database that should look like:
```
[
{
"paymentTypeId": 1,
   "paymentTypeName": "Visa",
   "AcctNumber": 1,
   "CustomerId": 2
 },
 {
   "paymentTypeId": 2,
   "paymentTypeName": "MasterCard",
   "AcctNumber": 1,
   "CustomerId": 2
 },
 {
   "paymentTypeId": 3,
   "paymentTypeName": "Discover",
   "AcctNumber": 1,
   "CustomerId": 2
}
]
```
To GET a specific, single payment type, add an /{id} to the end of the l`ocalhost:5000/PaymentTypes` URL. The result should only include the single payment type with the Id you added like the below:

```
[
{
"paymentTypeId": 3,
   "paymentTypeName": "Discover",
   "AcctNumber": 1,
   "CustomerId": 2
}
]
```
## POST
To POST a new object to your existing array for PaymentType, select POST, then paste `localhost:5000/PaymentTypes` into the field. Then click Body underneath the field, select raw, and then paste this below snippet or make one similar then click send. The result should be the new PaymentType you made:

```
{
	"paymentTypeName": "New Value"
}
```
## PUT
To update an existing PaymentType, select PUT then paste `localhost:5000/paymentTypes/2` or any other existing order. Then follow the same directions as the POST example, and change the values then click send:
```
{
	"paymentTypeName": "New Value"
}
```
You should get nothing back from this besides an OK status. When you run the GET query the computer you specified in your PUT query should show the updated, edited information you gave it.

## DELETE
To DELETE an existing product type, select DELETE then paste `localhost:5000/PaymentTypes/2` or any other existing PaymentType then click send. You should get nothing back from this besides an OK status. When you run the GET query the order with the Id you specified in your DELETE query should no longer exist.

# 3. Product
Use the command `dotnet run` to start the program, BangazonAPI. Once the program is running, open up the Postman desktop app and run the following commands for each request method:

`GET`
To GET all product types, select GET in Postman then paste `localhost:5000/Products` into the field and click send. The result should be an array of all the products in the database that should look like:
```
[
{
    "Id": 1,
   "Price": 1,
   "Title": "neato",
   "Description": "Rad",
   "Quantity": 2,
   "ProductTypeId": 2,
   "CustomerId": 2
 },
 {
   "Id": 2,
   "Price": 21,
   "Title": "aTitle,
   "Description": "Rad",
   "Quantity": 2,
   "ProductTypeId": 2,
   "CustomerId": 2
 },
 {
   "Id": 3,
   "Price": 1,
   "Title": "stuff",
   "Description": "Rad",
   "Quantity": 2,
   "ProductTypeId": 2,
   "CustomerId": 2
}
]
```
To GET a specific, single product type, add an /{id} to the end of the l`ocalhost:5000/products` URL. The result should only include the single product  with the Id you added like the below:

```
[
{
   "Id": 3,
   "Price": 1,
   "Title": "stuff",
   "Description": "Rad",
   "Quantity": 2,
   "ProductTypeId": 2,
   "CustomerId": 2
}
]
```
## POST
To POST a new object to your existing array for Product, select POST, then paste `localhost:5000/Products` into the field. Then click Body underneath the field, select raw, and then paste this below snippet or make one similar then click send. The result should be the new Product you made:

```
{
	"Product": "New Value"
}
```
## PUT
To update an existing PaymentType, select PUT then paste `localhost:5000/products/2` or any other existing order. Then follow the same directions as the POST example, and change the values then click send:
```
{
	"paymentTypeName": "New Value"
}
```
You should get nothing back from this besides an OK status. When you run the GET query the computer you specified in your PUT query should show the updated, edited information you gave it.

## DELETE
To DELETE an existing product type, select DELETE then paste `localhost:5000/products/2` or any other existing product then click send. You should get nothing back from this besides an OK status. When you run the GET query the order with the Id you specified in your DELETE query should no longer exist.
