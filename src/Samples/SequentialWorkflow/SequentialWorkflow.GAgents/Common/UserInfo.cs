namespace SequentialWorkflow.GAgents.Common;

[GenerateSerializer]
public class UserInfo
{
    [Id(0)] public string UserName { get; set; }
    [Id(1)] public int VipLv { get; set; }
    [Id(2)] public int Amount { get; set; }
    [Id(3)] public string ShippingAddress { get; set; }
}