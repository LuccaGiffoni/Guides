using Scripts_v2.Data.Models.Enums;

namespace Scripts_v2.Utilities
{
    public static class RoleManager
    {
        public static bool IsManager { get; private set; }

        public static void SetRole(EUserRole role)
        {
            IsManager = role == EUserRole.Manager;
        }
    }
}