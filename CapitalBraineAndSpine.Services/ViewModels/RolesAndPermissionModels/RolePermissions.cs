namespace CapitalBraineAndSpine.Services.ViewModels.RolesAndPermissionModels
{
    public class Role
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
    }

    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
