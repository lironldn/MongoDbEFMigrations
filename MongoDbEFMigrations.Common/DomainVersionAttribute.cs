namespace MongoDbEFMigrations.Common;

[AttributeUsage(AttributeTargets.Class,
        AllowMultiple = false, Inherited = false)
]
public class DomainVersionAttribute : Attribute
{
    public int Version { get; }

    public DomainVersionAttribute(int version)
    {
        Version = version;
    }

    public static int GetVersion<T>()
    {
        var attr = GetCustomAttributes(typeof(T))
            .FirstOrDefault(a => a is DomainVersionAttribute) ??
                   throw new EntityVersionConverterException($"No DomainVersionAttribute found on type {typeof(T).Name}");

        return ((DomainVersionAttribute)attr).Version;
    }
}

