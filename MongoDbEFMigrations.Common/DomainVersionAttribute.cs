namespace MongoDbEFMigrations.Common;

[System.AttributeUsage(AttributeTargets.Class,
        AllowMultiple = false, Inherited = false)
]
public class DomainVersionAttribute : Attribute
{
    public int Version;

    public DomainVersionAttribute(int version)
    {
        Version = version;

    }

    public int GetVersion() => Version;
}