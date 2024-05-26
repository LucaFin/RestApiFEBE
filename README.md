## Description

This is a full-stack application with a C# backend and an Angular frontend. The backend provides a RESTful API to manage users and todos, while the frontend consumes these APIs and displays the data.

## Technologies Used

- **Backend**: C# (.NET 8)
- **Frontend**: Angular
- **Testing**: xUnit, Moq, Serilog
- **Containerization**: Docker

## Setup Instructions

### Prerequisites

- .NET 8 SDK
- Node.js and npm
- Angular CLI
- Docker

## Backend
### Compile and Run

- Navigate to the Backend directory.

```
cd Backend
```

- Restore the dependencies and build the project.

```
dotnet restore
dotnet build
```

- Run the project.

```
dotnet run
```
### Running Tests
- Navigate to the Backend.Tests directory.

```
cd ../Backend.Tests
```

- Run the tests.

```
dotnet test
```

## Frontend
### Setup and Run
-Navigate to the Frontend directory.

```
cd ../Frontend
```

- Install the dependencies.

```
npm install
```

- Run the application.

```
ng serve
```

## Docker
### Building and Running with Docker Compose
- Build and run the containers.

```
docker-compose up --build
```

The backend will be available at http://localhost:8080 and the frontend at http://localhost:4200.

## Endpoints
### Users

- Retrieve paginated list of users.

```
GET /api/users?limit={limit}&offset={offset}
```
### Todos

- Retrieve paginated list of todos for a specific user.
```
GET /api/todos?userId={userId}&limit={limit}&offset={offset}
```
