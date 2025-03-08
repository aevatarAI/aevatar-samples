# Graph Rag Ingestion Sample

This sample demonstrates how to ingest data with Neo4j Store in agent.

## Agents

### SportsAgent
SportsAgent is a Neo4j graph database agent designed for sports domain data management. It implements:

- Data Ingestion: Dynamically creates nodes (Players/Clubs) and relationships (PLAYS_FOR/RIVAL) through CreateDataAsync 
- Query Interface: Provides sports-specific queries like retrieving a club's players (GetClubPlayers) and rival clubs (GetClubRivals)


## Running the Projects

### 1. neo4j deployment
This project utilizes Neo4j Desktop (local development edition) for graph data management. While Neo4j supports multiple deployment modes including Docker, Server, and Cloud (AuraDB), we recommend the following approach for this sample.

#### Neo4j Desktop

- Official development environment for macOS/Windows/Linux
- One-click local DBMS instance management
- Built-in browser interface
- Pre-configured settings matching this project

### 2. Add services config
Open silo's configuration file (../Samples/GraphIngestion/GraphIngestion.Silo/appsettings.json) and add the following config.

```json
{
  "Neo4j": {
  "Uri": "bolt://localhost:7687",
  "User": "neo4j",
  "Password": "****"
  }
}
```