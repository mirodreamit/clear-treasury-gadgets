# Introduction 
Gadgets API

# Getting Started
1.	In package manager console set the Project of the respective dbContext Repository as a default project (CT.Repository)

# How to Add Migrations [sql server]
Package manager console:

*** set CT.Repository as Default project in Package Manager Console   

    add-migration 'GD_Init' -Context 'GadgetsDbContext' -OutputDir 'Migrations\GD\SqlServer' -startupProject 'CT.Repository.Executor'
    add-migration 'IS_Init' -Context 'IsDbContext' -OutputDir 'Migrations\IdentityServer\SqlServer' -startupProject 'CT.Repository.Executor'
    
# How to update database [sql server]

Generate migrations using sql server dbcontext design time options.

*** set CT.Repository as Default project in Package Manager Console   

    Update-Database -args '"Server=localhost;Initial Catalog=gadgets-dev;Connection Timeout=30;Integrated Security=True;Encrypt=False"' -Context 'GadgetsDbContext' -startupProject 'CT.Repository.Executor'
    Update-Database -args '"Server=localhost;Initial Catalog=gadgets-dev;Connection Timeout=30;Integrated Security=True;Encrypt=False"' -Context 'IsDbContext' -startupProject 'CT.Repository.Executor'

# How to run the solution
    Update the database.
    Seed data (run Seed.sql from the MS Sql Management Studio).
    Right click on solution and click rebuild (this ensures that post build events of plugins are executed simulating manual plugin deploy).
    Set multiple startup projects to CT.Gadgets..FunctionApp and CT.Gadgets.SignalR.Host inside the _Apps solution folder.
    Run the solution.
    Search for swagger UI url in the Console (probably last url displayed).
    Ctrl Click the swagger UI.
    You have two swagger UIs displayed, one for the function API and other for the SignalR host.

# Swagger UI
Api routes follow REST principles. Using the swagger interface is straightforward.

# Solution Structure
This is an example of Clean Architecture CQRS (Command Query Responsibility Segregation) solution.
Thin clients (AzureFunction API project, Test project) call the main Application logic using the MediatR pattern.
Main business logic is modular and testabile.

# Solution Projects

## _Apps
Folder for the clients or our aplication. 
- an azure functions api is a client
- Windows worker (service) could be a client
- a simple console app can be a client

## Application
Main application logic.
Handles exceptions with a global handler.
Measures processing time of each request.
Validates requests using fluent validation rules.
Executes request handlers.
Does db CRUD operations.
Returns data to clients using standard output format, ensuring each response follows the same structure (good for frontend).

## Repository
Implemented Indexes, unique and non unique.
Disabled onDeleteCascade option.
There is a Seed.sql file in the Sql folder. After updating the database, you can run the file from MS SQL Server Management studio.

## Tests
Project: CT.Tests project. 
CQRS solution design pattern enables testing of the application logic oblivious to the client using it. Tests should only test the Application project.
They contain tests for scenarios described in the project documentation.

# Code Examples
Examples of typical scenarios for various standard operations.

## Transactional insert
Class: CreateGadgetFullCommandHandler
Scenarion: Objects for insert are prepared, transaction is opened in read commited isolation level. 
Data is inserted, table by table. If any operation fails, everything is rolled back.

## Paging data retrieval
Namespace: CT.FunctionApp.Functions.Questions
Class: CT.FunctionApp.Functions.Questions
Url parameter:
[QueryOpenApiParameter("paging", "{\"pageSize\":\"-1\",\"pageIndex\":\"0\"}", false, typeof(string))]
Example: {"pageSize":"10","pageIndex":"0"}

## Sorting data
Namespace: CT.FunctionApp.Functions.Questions
Class: CT.FunctionApp.Functions.Questions
Url parameter:
[QueryOpenApiParameter("sort", "[{\"fieldName\":\"fieldName\",\"direction\":\"asc|desc\"}}]", false, typeof(string))]
Example: [{"fieldName":"updatedAt","direction":"desc"}]
* it is possible to sort by more than one fieldName in given order

## Filtering data
Namespace: CT.FunctionApp.Functions.Questions
Class: CT.FunctionApp.Functions.Questions
Url parameter:
[QueryOpenApiParameter("filter", "[{\"fieldName\":\"fieldName\",\"filter\":[{\"op\":\"gt|lt|gte|lte|eq|startsWith|contains\",\"value\":\"some_value\"}]}]", false, typeof(string))]
Example: [{"fieldName":"name","filter":[{"op":"startsWith","value":"value"}]}]
* it is possible to filter by more than one field, logical AND is applied

# How to test SignalR hub
Open visual studio code in the folder CT.Gadgets.SignalR.Host
In the terminal run http-server, open the server url and go to page /testsignalr.html.
Using the swagger UI Increase the stock quantity of any gadget.
TestSignalR.html page updates the quantity.

# Best Practices
    * Clean architecture: CQRS solution design pattern is used. Easily testabile business logic, multiple clients are supported (function app or in the future a standard api, or a console app, etc). *
    * Authentication: JWT. Short lived access tokens are returned in the response so the frontend can store them in memory. Long lived refresh tokens are return as HttpOnly cookies.*
    * Thin clients: It is easy to migrate the entire app to another platform. Azure functions can be replaced by windows services if situation dictates. All logic is placed inside the Application project. *
    * Project: abstractions separated from implementations. Rule, abstraction projects don't have any project dependencies *
    * Swagger documentation: all parameters are clearly visible in the swagger UI page. Authentication parameter is also shown, but is not used in the solution. *
    * REST principles: all endpoints follow REST naming principles.*
    * Version control: all endpoints contain a version id in their url (v1). An endpoint of a newer version can easily be created. If possible the old one should not be deleted. *
    * Standard outputs: All endpoints return standardized output. Exceptions, validation errors and successful results are returned in this format (BaseOutput<T>). *
    * Request validation: CQRS feature file contains the request class, models for validation, a request handler and private methods. For brevity and ease of access all of this is placed in the same file. *
    * Global exception handling *
    * Data Integrity: Database contains foreign keys and onCascade delete is turned off. If parent entity needs to be delete, first every child needs to be deleted.*
    * Data Paging: Data retrieval has default sorting followed with the db index for speed and resource saving. *
    * Data Filtering: Data retrieval supports paging and filtering on the db level. This saves bandwith and speeds up the application. *
    * Data Sorting: Data retrieval supports sorting on the db level. Handled in a generic way. Sorting is by default possible on every column endpoint returns. *
    * Db indexes: some unique for integrity reasons and some for speedy data retrieval *
    * Unit test project: tests are create for business use cases, no need to test plumbing code. *
    * Data seeding - Seed.sql script is provided in the solution Sql folder. Can be run from MS SQL Mangement Studio *
    * Solution items: important external files are placed in the Solution item folder. *
