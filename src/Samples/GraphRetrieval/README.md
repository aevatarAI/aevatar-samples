# Graph Rag Integration Sample

This sample demonstrates how to integrate Graph Rag with Neo4j Store in agent.

## Agents

### MovieChatAgent
- **Role**: Inherits from `ChatAgent`, which is a specialized agent for handling user interaction with llm.



## Running the Projects

### 1. neo4j deployment
This project utilizes Neo4j Desktop (local development edition) for graph data management. While Neo4j supports multiple deployment modes including Docker, Server, and Cloud (AuraDB), we recommend the following approach for this sample.

#### Neo4j Desktop

- Official development environment for macOS/Windows/Linux
- One-click local DBMS instance management
- Built-in browser interface
- Pre-configured settings matching this project

### 2. Configure AI services config
Open silo's configuration file (../Samples/RouterWorkflow/src/RouterWorkflow.Silo/appsettings.json) and configure the AIServices section.

```json
"AIServices": {
  "AzureOpenAI": {
    "Endpoint": "",
    "ChatDeploymentName": "",
    "ApiKey": ""
  },
  "AzureOpenAIEmbeddings": {
    "Endpoint": "",
    "DeploymentName": "",
    "ApiKey": ""
  },
  "Neo4j": {
  "Uri": "bolt://localhost:7687",
  "User": "neo4j",
  "Password": "****"
  }
}
```