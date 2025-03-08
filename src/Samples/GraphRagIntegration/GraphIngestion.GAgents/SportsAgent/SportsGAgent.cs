using Aevatar.Core;
using Aevatar.Core.Abstractions;
using GraphIngestion.GAgents.Common;
using GraphIngestion.GAgents.Model;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;

namespace GraphIngestion.GAgents.SportsAgent;


[GenerateSerializer]
public class SportsGAgentState : StateBase
{
}

[GenerateSerializer]
public class SportsStateLogEvent : StateLogEventBase<SportsStateLogEvent>;


public interface ISportsGAgent : IStateGAgent<SportsGAgentState>
{
    Task CreateDataAsync(IEnumerable<Node> nodes, IEnumerable<Relationship> relationships);
    Task<List<string>?> GetClubPlayers(string clubName);
    Task<List<(string, string)>?> GetClubRivals(string clubName);
}

public class SportsGAgent : GAgentBase<SportsGAgentState, SportsStateLogEvent>, ISportsGAgent
{

    private readonly ILogger<SportsGAgent> _logger;
    private readonly IDriver _driver;

    public SportsGAgent(ILogger<SportsGAgent> logger, IDriver driver)
    {
        _logger = logger;
        _driver = driver;
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("");
    }
    
    public async Task CreateDataAsync(IEnumerable<Node> nodes, IEnumerable<Relationship> relationships)
    {
        try
        {
            var builder = new CypherQueryBuilder();
            var (query, parameters) = builder.Build(nodes, relationships);
            
            await using var session = _driver.AsyncSession();
            await session.ExecuteWriteAsync(async tx => 
            {
                await tx.RunAsync(query, parameters);
            });
        }
        catch (ClientException e)
        {
            _logger.LogError("Error storing, msg: {msg}, code: {code}", e.Message, e.Code);
        }
        catch (AuthenticationException e)
        {
            _logger.LogError("Error authentication, msg: {msg}, code: {code}", e.Message, e.Code);
        }
    }
    
    public async Task<List<string>?> GetClubPlayers(string clubName)
    {
        try
        {
            var query = @"
        MATCH (p:Player)-[:PLAYS_FOR]->(c:Club {Name: $clubName})
        RETURN p.Name AS PlayerName
        ORDER BY PlayerName";

            var parameters = new { clubName };

            await using var session = _driver.AsyncSession();
            return await session.ExecuteReadAsync(async tx => 
            {
                var result = await tx.RunAsync(query, parameters);
                return await result.ToListAsync(r => 
                    Neo4j.Driver.ValueExtensions.As<string>(r["PlayerName"])); 
            });
        }
        catch (ClientException e)
        {
            _logger.LogError("Error Quering, msg: {msg}, code: {code}", e.Message, e.Code);
            return null;
        }
        catch (AuthenticationException e)
        {
            _logger.LogError("Error authentication, msg: {msg}, code: {code}", e.Message, e.Code);
            return null;
        }
    }

    public async Task<List<(string, string)>?> GetClubRivals(string clubName)
    {
        try
        {
            var query = @"
        MATCH (c:Club {Name: $clubName})-[:RIVAL]-(rival:Club)
        RETURN c.Name AS Club, rival.Name AS RivalClub
        ORDER BY RivalClub";

            var parameters = new { clubName };
        
            await using var session = _driver.AsyncSession();
            return await session.ExecuteReadAsync(async tx => 
            {
                var result = await tx.RunAsync(query, parameters);
                return await result.ToListAsync(r => 
                (
                    Neo4j.Driver.ValueExtensions.As<string>(r["Club"]),   
                    Neo4j.Driver.ValueExtensions.As<string>(r["RivalClub"]) 
                ));
            });
        }
        catch (ClientException e)
        {
            _logger.LogError("Error Querying, msg: {msg}, code: {code}", e.Message, e.Code);
            return null;
        }
        catch (AuthenticationException e)
        {
            _logger.LogError("Error authentication, msg: {msg}, code: {code}", e.Message, e.Code);
            return null;
        }
        
    }
    
}