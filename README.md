# ECommerce API

Backend API for an e-commerce system built with ASP.NET Core Web API.

## Project Overview

This project provides the backend for an e-commerce application.
It manages users, schools, classes, products, categories, shopping carts, orders,
checkout, payments, delivery addresses, stock and authentication.

## Technologies Used

* ASP.NET Core Web API
* .NET 9
* Entity Framework Core
* SQL Server
* JWT Authentication
* BCrypt Password Hashing
* Gmail SMTP
* Swagger / OpenAPI

## Features

### Authentication & Authorization

* User registration
* User login
* JWT authentication
* Role-based authorization
* Admin and User roles
* Change password
* Forgot password
* Reset password
* Temporary reset tokens
* Reset token expiration
* One-time reset token usage

### Email

* Gmail SMTP integration
* Password reset email
* Password reset link
* Email-based password recovery
* Email credentials stored securely using .NET User Secrets

### Schools

* Create school
* Get schools
* Get school by ID
* Update school
* Delete school

### School Classes

* Create school class
* Get school classes
* Get school class by ID
* Update school class
* Delete school class

### Products

* Create product
* Get products
* Get product by ID
* Update product
* Delete product
* Product search
* Category filtering
* School filtering
* School class filtering
* Pagination
* Stock management

### Categories

* Create category
* Get categories
* Get category by ID
* Update category
* Delete category

### Shopping Cart

* Add products to cart
* Update cart quantity
* Remove cart items
* Stock validation

### Orders

* Create orders
* View user orders
* View order details
* Checkout
* Order status management
* Order items

### Payment

* Cash on Delivery (COD)
* Payment status tracking

### Delivery

* Delivery address management
* Full name
* Phone
* Address
* City

### Admin

* View users
* View user details
* Update user roles
* Delete users
* Manage products and categories
* Manage schools and classes

### Security & Error Handling

* JWT authentication
* Role-based authorization
* BCrypt password hashing
* Model validation
* Global exception middleware
* Database transactions for checkout
* Reset token expiration and validation

## Project Structure

```text
ECommerceAPI
├── Controllers
├── Data
├── DTOs
├── Middleware
├── Models
├── Services
├── Migrations
├── Program.cs
└── appsettings.json
```

## Database

The application uses SQL Server with Entity Framework Core.

Main entities:

* User
* School
* SchoolClass
* Product
* Category
* Cart
* CartItem
* Order
* OrderItem
* DeliveryAddress

## Password Reset Flow

```text
User enters email
        ↓
Backend generates temporary reset token
        ↓
Token is stored in database
        ↓
Reset link is sent through Gmail SMTP
        ↓
User opens reset link
        ↓
Angular reset page reads the token
        ↓
User enters new password
        ↓
Backend validates token
        ↓
Password is updated
        ↓
Reset token is cleared
        ↓
User is redirected to Login
```

## Email Configuration

Sensitive email credentials are stored using .NET User Secrets
and are not committed to the Git repository.

Non-sensitive SMTP settings are stored in `appsettings.json`.

```text
appsettings.json
├── SMTP Server
└── Port

User Secrets
├── Email
└── App Password
```

## Running the Project

1. Clone the repository.
2. Open the project in Visual Studio.
3. Configure the SQL Server connection string in `appsettings.json`.
4. Configure Gmail SMTP credentials using .NET User Secrets.
5. Apply migrations.
6. Run the project.
7. Open Swagger to test the API.
