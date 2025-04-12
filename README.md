# Calmska

## 📖 Main features of the app
- <strong>Mood tracker</strong>
- <strong>Pomodoro</strong>
- <strong>Hygge Tips</strong>

### 🏗️ Project Layout
- MAUI
- API
  - <strong>Program.cs</strong>: Configures application endpoints, dependency injection, AutoMapper, and repository setup.
  - <strong>CalmskaDbContext.cs</strong>: Manages database initialization and table definitions.
  - <strong>Properties</strong>: Includes launchSettings.json for environment-specific configurations.
  - <strong>DTO</strong>: Contains Data Transfer Objects (DTOs) for structured communication between API and clients.
  - <strong>Helpers</strong>: Utility classes and methods such as:
	- <strong>HashPassword</strong> for secure password management.
	- <strong>MapperProfiles</strong> for object-to-object mappings.
	- <strong>Pagination</strong> logic for API responses.
  - <strong>Interfaces</strong>: Defines contracts for dependency injection and ensures flexibility in implementation.
  - <strong>Repository</strong>: repositories with core database operations, implementing the Repository Pattern.
- Models:
  - Models: 
	- <strong>Entity Models</strong>: Defines the structure of database tables and entities.
	- <strong>MongoDbSettings</strong>: Configuration for MongoDB connection and settings.


### 🛠️ Tools
- <strong>MongoDB</strong>: database for scalable and flexible data storage.
- <strong>REST API</strong>: Clean and efficient APIs for client-server communication.
- <strong>MVVM</strong>: Model-View-ViewModel for robust application architecture.
- <strong>Dependency Injection</strong>: Achieves loosely coupled and testable code.
- <strong>Repository Pattern</strong>: Encapsulates data access logic and promotes cleaner architecture.
- <strong>DTO (Data Transfer Objects)</strong>: Streamlines communication between layers.
- <strong>AutoMapper</strong>: Simplifies object mapping and transformation.
- <strong>Pagination</strong>: Efficient handling of large datasets in API responses.

#### ➕ Additional Features

- Notifications
- Customisation settings
- AI mood analizer:
	1. Request to the Fine-Tuned llm to recognize emotion from the provided text.
	2. Save emotion in the database.
	3. Get emotions from the database from last week.
	4. Request second llm to the advices based on the provided emotions.
	5. Show the advices to the user.

[🖌 Project Design](https://www.figma.com/design/mdhVEHFrAAc71qLnXgYBFo/Calmska?node-id=0-1&t=RiXjXKAvAoGOzCzG-1) 

[💿 Database Schema](https://www.figma.com/design/KHtrSLFCdqJfANaMcqE7qa/Relational-Database-Diagram---Component-Kit-(Community)?node-id=3-728&t=izB1EdeXBzwRAZs7-1)

##### 📝Todo
- Mood history list