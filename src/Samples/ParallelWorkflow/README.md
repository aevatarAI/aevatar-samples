# Agent Orchestration System

This sample orchestrates two GAgents to collect user transfer data and utilizes parallel execution to improve execution efficiency.

## Agents

### 1. FetchGAgent
- **Role**: Get transaction results by height
- **Event Handler**:
    - `FetchEvent`, When receiving `FetchEvent`, `FetchGAgent` will query transaction results according to the height, and then publishes `AggregationEvent`.

### 2. AggregationGAgent
- **Role**: Filter and aggregate transfer data
- **Event Handler**:
    - `AggregationEvent`, When receiving `AggregationEvent`, `AggregationGAgent` will filter and aggregate the data, and store the result for query.

## Relationships between agents
    `GroupGAgent` --> `FetchGAgent` --> `AggregationGAgent`

## Running the Projects

### Start the Silo Project

Navigate to the Silo project directory and run the following command:

```sh
dotnet run --project ParallelWorkflow.Silo
```

### Start the Client Project

Navigate to the Client project directory and run the following command:

```sh
dotnet run --project ParallelWorkflow.Client
```

By following these steps, you will be able to start the Silo Host and interact with GAgents using the Client project.

## Technologies Used
- [.NET Core](https://dotnet.microsoft.com/)
- [Orleans](https://dotnet.github.io/orleans/)
- [AevatarAI](https://aevatar.ai/)