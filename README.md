# KafeHaruno ☕

KafeHaruno is a web-based cafe management system focused on backend development, specifically managing customer orders, tables, and menu items (like different types of coffee). The project is designed to help improve skills in handling relationships between entities, building APIs, and implementing authentication.

## Project Overview

- **Backend:** .NET Core for API development and Entity Framework Core for database management.
- **Frontend:** A simple UI built using ASP.NET Core Razor Pages to interact with the backend.
- **Authentication:** Token-based authentication using Bearer tokens to control access.
- **Database:** Uses SQL Server and Entity Framework for managing data, with a focus on relationships like many-to-many between tables and orders.

### Features

- **Table Management:** View and select tables for customers (e.g., Table 1 to Table 21).
- **Order System:** Place, update, or delete coffee orders for customers.
- **Role-Based Pages:** Different views for waitstaff and admins, providing tailored functionality for each role.
- **Authentication:** Secure access using token-based authentication for managing roles and data.
- **Responsive UI:** The project includes a simple, clean interface for selecting tables and managing orders.
- **Data Handling:** Supports updating, adding, and deleting records in the database for orders, coffees, and customer data.

### Project Structure

- **Controllers:** Handle API requests and manage logic for coffee, orders, tables, etc.
- **Models:** Represent the entities in the system such as `Order`, `Coffee`, `Table`, and `Customer`.
- **Services:** Business logic is encapsulated in services for clean separation of concerns.
- **Validators:** Input validation using custom validators to ensure correct data is processed.
- **Token Authentication:** Bearer tokens are used for user login and role management.

## Technologies

- **ASP.NET Core**: Framework for building the backend API and frontend Razor Pages.
- **Entity Framework Core**: ORM for handling database operations.
- **SQL Server**: The database used to store orders, tables, and coffee details.
- **JWT (JSON Web Tokens)**: Token-based authentication for securing API endpoints.
- **Bootstrap**: Used for basic styling of the frontend pages.

## Usage

### Admin Role:
 - Admin users can manage all tables, orders and menu items. They can only create a new order, mark existing bills as paid, and delete existing ones. They can create or assign new waiters or managers.

### Waiter/Waitress Role:
 - Waiters can only manage tables and orders. They can't access management features. They can change their username or password, create a new order but can't delete it. They can't see billing information.

## Future Enhancements
 - **Adding detailed analysis for order management and sales.**-
 - **Adding end of day and similar functions, refreshing the database every 2 days.**-
 - **Making the UI more mobile friendly.**-
 - **About In this project, I did it to further develop my backend skills and gain more experience in relationships, and I created the frontend part in a basic structure. I WILL CONTINUE TO DEVELOP THE PROJECT .**-

## Contributions
 -**Feel free to fork the project and submit pull requests. Open issues for any bugs or feature requests!**-

## Setup Instructions

1. Clone the repository:
   ```bash
   git clone https://github.com/theharuun/KafeHaruno.git
2. Open the solution in Visual Studio.
3. Install required dependencies and build the project.
4. Set up the SQL Server database connection string in the appsettings.json file:
   ```bash
    "ConnectionStrings": {
    "DefaultConnection": "Server=your_server;Database=KafeHarunoDB;Trusted_Connection=True;"
    }

5.Run the migrations to set up the database  and Launch the project:
   ```bash2
         "dotnet ef database update "
         "dotnet run"


  
