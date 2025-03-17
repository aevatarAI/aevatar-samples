using Aevatar.PermissionManagement;
using Volo.Abp.Authorization.Permissions;

namespace Permission.Silo;

public class AevatarPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var permissionInfos = GAgentPermissionHelper.GetAllPermissionInfos();
        var permissionInfoMap = new Dictionary<string, List<string>>();
        permissionInfos.ForEach(t =>
        {
            var list = permissionInfoMap.GetOrDefault(t.GroupName);
            if (list.IsNullOrEmpty())
            {
                permissionInfoMap.Add(t.GroupName, new List<string>(){t.Name});
            }
            else
            {
                list.AddIfNotContains(t.Name);
            }
        });
        foreach (var permissionInfo in permissionInfoMap)
        {
            var group = context.AddGroup(permissionInfo.Key);
            foreach (var permission in permissionInfo.Value)
            {
                group.AddPermission(permission);
            }
        }
    }
}