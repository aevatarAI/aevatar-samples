# Agent Orchestration System

This project orchestrates three specialized agents to handle user validation, order processing, and shipping in sequence. Designed for e-commerce transaction workflows.

## Agents

### 1. ValidateUserGAgent
- **Role**: Initial gatekeeper for user authentication
- **Event Handler**:
    - `ValidateUserEvent`, When receiving a `ValidateUserEvent`, ValidateUserGAgent will verify whether the user meets the required VIP level threshold.
       Upon successful validation, the agent will trigger a `ValidateUserPassEvent` containing the user's authorization context.

### 2. ProcessOrderGAgent
- **Role**: Order fulfillment manager
- **Event Handler**:
    - `ValidateUserPassEvent`, When receiving the `ValidateUserPassEvent`, `ProcessOrderGAgent` will validate the user's payment eligibility and available funds for the requested purchase.
       Upon successful validation, the agent will trigger a `ProcessOrderEvent` event.

### 3. ShippingGAgent
- **Role**: Logistics coordinator
- **Event Handler**:
    - `ProcessOrderEvent`, When receiving the `ProcessOrderEvent`, `ShippingGAgent` will validate the user's shipping address。

## Relationships between agents
    `GroupGAgent` --> `ValidateUserGAgent` --> `ProcessOrderGAgent` --> `ShippingGAgent`

## Running the Projects

### Start the Silo Project

Navigate to the Silo project directory and run the following command:

```sh
dotnet run --project SequentialWorkflow.Silo
```

### Start the Client Project

Navigate to the Client project directory and run the following command:

```sh
dotnet run --project SequentialWorkflow.Client
```

By following these steps, you will be able to start the Silo Host and interact with GAgents using the Client project.

## Technologies Used
- [.NET Core](https://dotnet.microsoft.com/)
- [Orleans](https://dotnet.github.io/orleans/)
- [AevatarAI](https://aevatar.ai/)