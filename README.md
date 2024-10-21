# **Clean Architecture with .NET WebAPI**

This template was developed based on Clean Architecture, using the powerful .NET framework. Now you can download the template and start your project without worrying about initial configurations, plugins, or project dependencies. Everything is already set up for you!

## 🧪 **Tests**
The project includes a ready-to-use testing layer so you can configure it according to your needs.

## 📁 **Project Structure**
**API**

👉 **Controllers:** The location where the application endpoints are.

👉 **SwaggerExtension:** Detailed configuration of Swagger and JWT.

**Application**

👉 **Commands and Queries:** Implementation of the CQRS pattern.


👉 **Notification:** Structure to notify the API about internal errors.


👉 **Validators:** Validations using FluentValidation.


👉 **DependencyInjectionApplication:** Responsible for the dependency injection (DI) of the Application layer.

**Core**

👉 **Entities:** The core of the application, containing all the entities that represent the domain.

👉 **Auth:** Structure for managing JWT information.

👉 **Enums:** Enumerators for status handling.

👉 **Repositories:** Interfaces for the repositories.

**Infrastructure**

👉 **Auth:** Authorization and authentication structure.

👉 **Persistence - Configuration:** Configuration of Entity Framework for the SQL Server database.

👉 **Persistence - Repositories:** Implementations of repositories for data persistence.

👉 **DependencyInjectionInfrastructure:** Responsible for the dependency injection (DI) of the Infrastructure layer.

## **Resources**

✅ Project structured with Clean Architecture.

✅ Developed with .NET 8.

✅ API Web.

✅ Entity Framework Core.

✅ Validation with FluentValidation.

✅ Documentation with Swagger.

✅ Repository Pattern.

✅ Unit Tests.

✅ CQRS Architectural Pattern.

✅ Logs with Serilog.

✅ Authentication and Authorization with Microsoft Identity and JWT.

