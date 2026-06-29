# GatherUp - Event Management Backend API

## Project Overview
GatherUp is a robust Backend management engine designed to streamline group event planning and budget tracking[cite: 13]. The system provides a centralized API for handling complex event logistics, ensuring data integrity and efficient performance for multi-user coordination[cite: 13].

## Technical Stack
* **Language:** C#[cite: 13]
* **Framework:** .NET / ASP.NET Core[cite: 13]
* **Architecture:** MVC Pattern, Repository Pattern[cite: 13]
* **Data Persistence:** XML-based storage[cite: 13]
* **Development Tools:** Visual Studio, Git, Postman[cite: 13]

## Architecture & Key Features
* **Repository Pattern:** Implemented to decouple data access logic from business logic, ensuring a clean and maintainable codebase[cite: 13].
* **Asynchronous Processing:** Utilized `async/await` patterns across REST endpoints to optimize server throughput and handle concurrent requests effectively[cite: 13].
* **Robust Data Management:** Developed custom generic repositories to handle CRUD operations on event and budget data stored in XML formats[cite: 13].
* **Scalable API Design:** Engineered a clean, modular API surface that facilitates easy integration with frontend interfaces[cite: 13].

## Technical Challenges & Solutions
* **Challenge:** Implementing efficient data persistence without a relational database while maintaining performance[cite: 13].
* **Solution:** Designed a generic XML-based repository layer that allows for fast serialization/deserialization, achieving sub-100ms response times for core data retrieval[cite: 13].

## Installation
1. Clone the repository: `git clone https://github.com/odelia50872/GatherUp`[cite: 13]
2. Open the solution in Visual Studio[cite: 13].
3. Restore NuGet packages[cite: 13].
4. Run the application to start the local API server[cite: 13].

## Future Improvements
* Migration from XML to a SQL database (MySQL) to enhance query performance and scalability[cite: 13].
* Implementation of automated unit tests to ensure stability across network edge cases[cite: 13].
