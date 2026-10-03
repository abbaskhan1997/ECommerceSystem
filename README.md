# ECommerce API

Backend API for an e-commerce system built with ASP.NET Core Web API.

## Project Overview

This project provides the backend for an e-commerce application.
It manages users, products, categories, shopping carts, orders,
checkout, payments, delivery addresses, stock and authentication.

## Technologies Used

- ASP.NET Core Web API
- .NET 9
- Entity Framework Core
- SQL Server
- JWT Authentication
- BCrypt Password Hashing
- Swagger / OpenAPI

## Features

### Authentication & Authorization
- User registration
- User login
- JWT authentication
- Role-based authorization
- Admin and User roles
- Change password
- Forgot password
- Reset password

### Products
- Create product
- Get products
- Get product by ID
- Update product
- Delete product
- Product search
- Category filtering
- Pagination
- Stock management

### Categories
- Create category
- Get categories
- Get category by ID
- Update category
- Delete category

### Shopping Cart
- Add products to cart
- Update cart quantity
- Remove cart items
- Stock validation

### Orders
- Create orders
- View user orders
- View order details
- Checkout
- Order status management
- Order items

### Payment
- Cash on Delivery (COD)
- Payment status tracking

### Delivery
- Delivery address management
- Full name
- Phone
- Address
- City

### Admin
- View users
- View user details
- Update user roles
- Delete users
- Manage products and categories

### Security & Error Handling
- JWT authentication
- Role-based authorization
- BCrypt password hashing
- Model validation
- Global exception middleware
- Database transactions for checkout

## Project Structure

ECommerceAPI
├── Controllers
├── Data
├── Middleware
├── Models
├── Services
├── Migrations
├── Program.cs
└── appsettings.json

## Database

The application uses SQL Server with Entity Framework Core.

Main entities:

- User
- Product
- Category
- Cart
- CartItem
- Order
- OrderItem
- DeliveryAddress

## Running the Project

1. Clone the repository.
2. Open the project in Visual Studio.
3. Configure the SQL Server connection string in `appsettings.json`.
4. Apply migrations.
5. Run the project.
6. Open Swagger to test the API.


