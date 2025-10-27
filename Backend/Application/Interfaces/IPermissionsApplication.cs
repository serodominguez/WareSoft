namespace Application.Interfaces
{
    public interface IPermissionsApplication
    {
        Task<bool> UserPermissionAsync(int userId, string moduleName, string actionName);
    }
}
