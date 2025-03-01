using Aevatar.Core.Abstractions;
using Aevatar.GAgents.ChatAgent.Dtos;
using Aevatar.GAgents.ChatAgent.GAgent;
using Aevatar.GAgents.ChatAgent.GAgent.SEvent;
using Aevatar.GAgents.ChatAgent.GAgent.State;

namespace GraphRagIntegration.GAgents.MovieChatAgent;

public interface IMovieChatGAgent : IChatAgent
{
    
}

public class MovieChatGAgent : ChatGAgentBase<ChatGAgentState, ChatGAgentLogEventBase, EventBase, ChatConfigDto>, IMovieChatGAgent
{

    
}