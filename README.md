P1118231 BENJAMIN SSETTUBA
ThamcoProfiles MVC
follow this URL thmcoprofiles-gdf4cycsdea7ezf9.uksouth-01.azurewebsites.net for the application

ThamcoProfiles is a web application built with ASP.NET Core MVC, designed to manage user profiles and product data. It integrates with Auth0 for user authentication, uses SQL Server for the database, and makes use of Polly for resilient HTTP calls. The project is structured to support both local development and production environments, with various services for profile management and product data integration.

Features

User Authentication with Auth0: Secure login and authentication with the ability to manage user profiles.
Product Integration: Integrates with an external product service to retrieve and display products.
Responsive UI: Built with Bootstrap, ensuring a mobile-friendly experience.
Error Handling: Uses Polly for retry policies and resilient HTTP calls.
Localization: International telephone numbers are supported using the intl-tel-input library.
Database Management: Uses Entity Framework Core to interact with SQL Server or SQLite (based on environment).
Technologies Used

ASP.NET Core MVC: Web framework for building the application.
Auth0: Authentication and authorization service for managing users.
Entity Framework Core: ORM for accessing the database (SQL Server in production, SQLite in development).
Bootstrap: Front-end framework for responsive design.
Polly: A resilience and transient-fault-handling library for HTTP calls.
Polly.Extensions.Http: Integration with HTTP client for retry and circuit breaker policies.
intl-tel-input: A library for formatting and validating international telephone numbers.
Setup and Installation

Prerequisites
.NET 6 or higher
Visual Studio or Visual Studio Code
SQL Server or SQLite
Auth0 Account

Clone the repository 
-git clone https://github.com/p1118231/ThamcoProfileService.git
-cd thamcoprofiles

Configuration
Auth0 Configuration: Set up an Auth0 account, create a new application, and configure the Auth0:Domain, Auth0:ClientId, and Auth0:ClientSecret in your appsettings.json file.
Database Configuration:
For Development: By default, the application uses SQLite for local development. No additional configuration is required.
For Production: Set the connection string for SQL Server in azure .

Install dependencies 
-dotnet restore

build the project
-dotnet build

run the project 
-dotnet run 

Accessing the Application
Visit the application at (https://thmcoprofiles-gdf4cycsdea7ezf9.uksouth-01.azurewebsites.net) to access the MVC application.
For authentication, sign in via Auth0 using your configured account.

Features
User Profile Management
Users can log in via Auth0 and view/update their profile.
If no user exists with the given Auth0 ID, a new user will be created in the database.
Product Integration
Fetches product data from an external service and displays it on the homepage.
Supports basic CRUD operations for profule data .
Telephone Number Validation
The intl-tel-input library is used to validate and format telephone numbers for international use.
Error Handling with Polly
The application uses Polly to manage transient errors in HTTP calls (such as when fetching products from an external service).
Retry policies and circuit breakers are applied to handle network issues more gracefully.

Deployment
To deploy the application to a production environment, configure the connection string and Auth0 settings appropriately. The application supports deployment to Azure or any other hosting service that supports ASP.NET Core.
The application also uses github to deploy to azure 

Troubleshooting
Authentication Issues
Make sure the Auth0 settings (Auth0:Domain, Auth0:ClientId, Auth0:ClientSecret) are correct.
Ensure that your callback URL is correctly configured in your Auth0 application settings.
Database Connection Issues
Ensure the correct connection string is specified for either SQLite (development) or SQL Server (production).
Make sure the database is accessible and the AccountContext is properly configured.
Application Crashes
Check the logs for detailed error messages.
For HTTP errors, ensure that the external services are reachable, and check the Polly configuration.

