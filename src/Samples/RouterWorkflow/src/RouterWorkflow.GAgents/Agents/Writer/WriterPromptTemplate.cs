namespace RouterWorkflow.GAgents.Agents.Writer;

public class WriterPromptTemplate
{
    public const string Prompt =
        @"
You are a writer and you need to output articles based on the content provided.

### The content:
{CONTENT}
";
}