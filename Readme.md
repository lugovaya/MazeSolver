# MazeSolver

MazeSolver is a .NET 8 application that provides an simple API for solving mazes in specific srting format.
At the moment it includes various services to handle maze solving requests using BFS and DFS algorithms.
Currently, the application supports only one maze at a time and stores the maze in memory.

## Projects

- **MazeSolver.Api**: The main API project that exposes endpoints for maze solving.
- **MazeSolver.Domain**: Contains the core domain logic and services for maze solving.

## Getting Started

### Prerequisites

- .NET 8 SDK

### Building the Solution

To build the solution, run the following command in the root directory:
```dotnet build```

### Running the Application

To run the application, use the following command:
```dotnet run --project MazeSolver.Api```

### Running Tests

To run the tests, use the following command:
```dotnet test```


## API Endpoints

### MazeController

- **GET /api/mazes**: Retrieves a mazes if any.
``````
curl -X 'GET' '.../api/mazes' -H 'accept: */*'

Response Code: 200
Response Body:
[
    {
        "id": "guid",
        "maze": "string",
        "solution": "string"
    }
]
``````
- **POST /api/mazes**: Solves a given maze, and if the solution is found it submits the maze into InMemory data store.
``````
curl -X 'POST' \
  '.../api/mazes' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "content": "string"
}'

Response Code: 201
Response Body: 
{
    "solution": "string"
}

Response Code: 404
Response Body: "Solution for provided maze was not found."

Response Code: 400
Response Body: 
{
    "errorMessage": "string"
}
``````
## Middleware

- **TimeoutMiddleware**: Handles configurable request timeouts.
- **ExceptionHandlerMiddleware**: Handles exceptions and returns appropriate error responses.

## Services

- **IMazeSolver**: Interface for maze solving algorithms.
- **IMazeService**: Interface for maze-related operations.
- **IRepository**: Interface for data access.

## Validators

- **MazeRequestValidator**: Contains validation rules for maze solving requests (according to the specific input format).

------------
### TODO:
- [ ] Add configurable timeout
- [ ] Add lazy initialization with static field to ensure thread safety in singleton
- [ ] Consider making ```IMazeSolver``` generic, e.g. ```<TParam, TResult>```
- [ ] Consider to introduce an ```OperationResult<T>``` type to return the result and the error message in ```MazeService```
- [ ] Consider to introduce ```Contracts``` to replace ```MazeConfigurationBase``` and use record types