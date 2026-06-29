# GatherUp - Event Management Backend API

## Project Overview
GatherUp is a robust Backend management engine designed to streamline group event planning and budget tracking. The system provides a centralized API for handling complex event logistics, ensuring data integrity and efficient performance for multi-user coordination.

## Technical Stack
* **Language:** C#
* **Framework:** .NET / ASP.NET Core
* **Architecture:** MVC Pattern, Repository Pattern
* **Data Persistence:** XML-based storage
* **Development Tools:** Visual Studio, Git, Postman

## Architecture & Key Features
* **Repository Pattern:** Implemented to decouple data access logic from business logic, ensuring a clean and maintainable codebase.
* **Asynchronous Processing:** Utilized `async/await` patterns across REST endpoints to optimize server throughput and handle concurrent requests effectively.
* **Robust Data Management:** Developed custom generic repositories to handle CRUD operations on event and budget data stored in XML formats.
* **Scalable API Design:** Engineered a clean, modular API surface that facilitates easy integration with frontend interfaces.

## Technical Challenges & Solutions
* **Challenge:** Implementing efficient data persistence without a relational database while maintaining performance.
* **Solution:** Designed a generic XML-based repository layer that allows for fast serialization/deserialization, achieving sub-100ms response times for core data retrieval.

## Installation
1. Clone the repository: `git clone https://github.com/odelia50872/GatherUp`
2. Open the solution in Visual Studio.
3. Restore NuGet packages.
4. Run the application to start the local API server.

## Future Improvements
* Migration from XML to a SQL database (MySQL) to enhance query performance and scalability.
* Implementation of automated unit tests to ensure stability across network edge cases.
