namespace ParallelWorkflow.GAgents.Agents;

[GenerateSerializer]
public class TransferInfo
{
    [Id(0)]
    public string From { get; set; }
    [Id(1)]
    public string To { get; set; }
    [Id(2)]
    public long Amount { get; set; }
}