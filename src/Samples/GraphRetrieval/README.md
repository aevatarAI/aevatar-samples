# Graph Rag Integration Sample

This sample demonstrates how to integrate Graph Rag with Neo4j Store in agent.

## Agents

### GraphRetrievalAgent
A basic agent which integrates with Neo4j data retrieval capabilities. It first asks an LLM to generate a Cypher query to fetch the exact information required to answer the question from the database. Then this query is executed and the resulting records are added to the context for the LLM to write the answer to the initial user question.



## Running the Projects

### 1. Neo4j deployment
This project utilizes Neo4j Desktop (local development edition) for graph data management. While Neo4j supports multiple deployment modes including Docker, Server, and Cloud (AuraDB), we recommend the following approach for this sample.

#### Neo4j Desktop

- Official development environment for macOS/Windows/Linux
- One-click local DBMS instance management
- Built-in browser interface
- Pre-configured settings matching this project

Make sure you have the movie dataset installed.

### 2. Qdrant deployment
This project utilizes Qdrant for vector store, as it is part of the ai base agent. Follow the instructions in the official documentation to set up and run Qdrant. https://qdrant.tech/documentation/quickstart/

### 3. Add AI services and neo4j config
Open silo's configuration file (../Samples/GraphRetrieval/GraphRetrieval.Silo/appsettings.json) and add the following configuration.

```json
{
  "SystemLLMConfigs": {
    "OpenAI": {
      "ProviderEnum": "Azure",
      "ModelIdEnum": "OpenAI",
      "ModelName": "gpt-4o",
      "Endpoint": "xxx",
      "ApiKey": "xxx"
    }
  },
  "AIServices": {
    "AzureOpenAIEmbeddings": {
      "Endpoint": "xxx",
      "DeploymentName": "xxx",
      "ApiKey": "xxxx"
    },
    "OpenAI": {
      "ModelId": "gpt-4o",
      "ApiKey": "xxx",
      "OrgId": null
    },
    "OpenAIEmbeddings": {
      "ModelId": "text-embedding-3-small",
      "ApiKey": "xxx",
      "OrgId": null
    }
  },
  "VectorStores": {
    "AzureAISearch": {
      "Endpoint": "http://localhost:6334/",
      "ApiKey": ""
    },
    "AzureCosmosDBMongoDB": {
      "ConnectionString": "",
      "DatabaseName": ""
    },
    "AzureCosmosDBNoSQL": {
      "ConnectionString": "",
      "DatabaseName": ""
    },
    "Qdrant": {
      "Host": "localhost",
      "Port": 6334,
      "Https": false,
      "ApiKey": ""
    },
    "Redis": {
      "ConnectionConfiguration": "localhost:6379"
    },
    "Weaviate": {
      "Endpoint": "http://localhost:8080/v1/"
    }
  },
  "Rag": {
    "AIChatService": "AzureOpenAI",
    "AIEmbeddingService": "AzureOpenAIEmbeddings",
    "BuildCollection": false,
    "DataLoadingBatchSize": 10,
    "DataLoadingBetweenBatchDelayInMilliseconds": 1000,
    "VectorStoreType": "Qdrant"
  },
  "Neo4j": {
    "Uri": "bolt://localhost:7687",
    "User": "neo4j",
    "Password": "12345678ß"
  }
}
```