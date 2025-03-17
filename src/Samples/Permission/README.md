# GAgents Permissions System

This project create three agents、four roles、four users， different user has different role(users may have multiple roles
), different role has different permission.

## Agents

### 1. MasterGAgent
classPermission Admin.Read and methodPermissions Admin.Write
### 2. DeveloperGAgent
methodPermissions Developer.View\Developer.Create\Developer.Delete
### 3. MasterGAgent
methodPermissions Master.View\Master.Create\Master.Delete

## Roles
four roles created in PermissionGAgentTestHostedService by roleManager.
grant permissions by IPermissionManager.SetForRoleAsync method
### 1. Admin
### 2. Master
### 3. Developer
### 4. Guest

## Running the Projects

### Start the Silo Project

Navigate to the Silo project directory and run the following command:

```sh
dotnet run --project Permission.Silo
```

### Start the Client Project

Navigate to the Client project directory and run the following command:

```sh
dotnet run --project Permission.Client
```

By following these steps, you will be able to start the Silo Host and interact with GAgents using the Client project.

## Technologies Used
- [.NET Core](https://dotnet.microsoft.com/)
- [Orleans](https://dotnet.github.io/orleans/)
- [AevatarAI](https://aevatar.ai/)