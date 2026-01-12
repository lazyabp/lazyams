namespace Lazy.Core.LazyAttribute;

public class DBSeedDataOrderAttribute : Attribute
{
    public int Order { get; set; }
    public DBSeedDataOrderAttribute(int order)
    {
        this.Order = order;
    }
}
