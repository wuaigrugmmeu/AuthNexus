namespace AuthNexus.SharedKernel.Constants
{
    /// <summary>
    /// 权限常量定义
    /// </summary>
    public static class PermissionConstants
    {
        // 用户管理权限
        public const string ViewUsers = "users:view";
        public const string CreateUsers = "users:create";
        public const string UpdateUsers = "users:update";
        public const string DeleteUsers = "users:delete";
        
        // 角色管理权限
        public const string ViewRoles = "roles:view";
        public const string CreateRoles = "roles:create";
        public const string UpdateRoles = "roles:update";
        public const string DeleteRoles = "roles:delete";
        
        // 权限管理权限
        public const string ViewPermissions = "permissions:view";
        public const string AssignPermissions = "permissions:assign";
        
        // 应用管理权限
        public const string ViewApplications = "applications:view";
        public const string CreateApplications = "applications:create";
        public const string UpdateApplications = "applications:update";
        public const string DeleteApplications = "applications:delete";
    }
}