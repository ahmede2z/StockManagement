# Stock Management System

A comprehensive inventory and order management solution designed to streamline product tracking and order processing for businesses of all sizes.

## Overview

The Stock Management System is a robust web application that enables businesses to efficiently manage their inventory and orders. The system provides an intuitive interface for tracking products, managing stock levels, and processing customer orders, all while maintaining data integrity and providing meaningful insights.

## Features

- **Product Management**: Create, view, update, and delete products with details such as name, price, and stock quantity
- **Order Processing**: Create and manage orders with multiple product items
- **Stock Tracking**: Automatically update stock levels when orders are placed
- **User-Friendly Interface**: Intuitive UI with responsive design for desktop and mobile devices
- **Error Handling**: Comprehensive error handling with user-friendly messages
- **Data Validation**: Client and server-side validation to ensure data integrity

## Technology Stack

- **Backend**: ASP.NET Core 8.0 MVC
- **Database**: Entity Framework Core with SQL Server
- **Frontend**: Bootstrap 5, HTML5, CSS3, JavaScript
- **Styling**: Bootstrap Icons for consistent iconography
- **Validation**: Data annotations and client-side validation
- **Logging**: Built-in ASP.NET Core logging infrastructure

## Architecture & Design Patterns

The application follows a clean architecture approach with separation of concerns:

- **Domain-Driven Design**: Core entities represent the business domain
- **Repository Pattern**: Abstracts data access logic from business logic
- **Unit of Work Pattern**: Ensures transaction integrity when working with multiple repositories
- **Service Layer Pattern**: Encapsulates business logic and orchestrates operations
- **MVC Pattern**: Separates the application into Model, View, and Controller components
- **DTO Pattern**: Data Transfer Objects for safe data exchange between layers

## Project Structure

- **StockManagement.Core**: Contains domain entities and repository interfaces
- **StockManagement.Infrastructure**: Implements data access with Entity Framework Core
- **StockManagement.Services**: Contains business logic and service implementations
- **StockManagement.Presentation**: MVC application with controllers and views

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- SQL Server (LocalDB, Express, or full edition)
- Visual Studio 2022 or Visual Studio Code

### Installation

1. Clone the repository

   ```
   git clone https://github.com/yourusername/StockManagement.git
   ```

2. Navigate to the project directory

   ```
   cd StockManagement
   ```

3. Restore dependencies

   ```
   dotnet restore
   ```

4. Update the database connection string in `appsettings.json` if needed

5. Apply database migrations

   ```
   dotnet ef database update --project StockManagement.Infrastructure --startup-project StockManagement.Presentation
   ```

6. Run the application

   ```
   dotnet run --project StockManagement.Presentation
   ```

7. Open your browser and navigate to `https://localhost:5001` or `http://localhost:5000`
