namespace Lazy.Shared;

public class PermissionConsts
{
    public const string PermissionManagement = "Permission";

    public class User
    {
        public const string Default = PermissionManagement + ".User";
        public const string Add = Default + ".Add";
        public const string Search = Default + ".Search";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public class Role
    {
        public const string Default = PermissionManagement + ".Role";
        public const string Add = Default + ".Add";
        public const string Search = Default + ".Search";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public class Menu
    {
        public const string Default = PermissionManagement + ".Menu";
        public const string Add = Default + ".Add";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public class Config
    {
        public const string Default = PermissionManagement + ".Config";
        public const string Update = Default + ".Update";
    }

    public class File
    {
        public const string Default = PermissionManagement + ".File";
        public const string Upload = Default + ".Upload";
    }

    public class Carousel
    {
        public const string Default = PermissionManagement + ".Carousel";
        public const string Add = Default + ".Add";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }
}
