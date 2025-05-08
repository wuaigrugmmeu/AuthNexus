namespace AuthNexus.SharedKernel.Interfaces
{
    /// <summary>
    /// 当前用户服务接口
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// 获取当前用户ID
        /// </summary>
        Guid? UserId { get; }
        
        /// <summary>
        /// 获取当前用户名
        /// </summary>
        string? Username { get; }
        
        /// <summary>
        /// 检查当前用户是否拥有指定权限
        /// </summary>
        /// <param name="permission">权限名称</param>
        /// <returns>是否有此权限</returns>
        bool HasPermission(string permission);
        
        /// <summary>
        /// 获取当前用户所有角色
        /// </summary>
        /// <returns>角色列表</returns>
        IEnumerable<string> GetRoles();
        
        /// <summary>
        /// 获取当前用户所有权限
        /// </summary>
        /// <returns>权限列表</returns>
        IEnumerable<string> GetPermissions();
    }
}