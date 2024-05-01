using Xtensive.Orm;

namespace Server.Persistence;

/// <summary>
///     Provide a reference to the <see cref="Domain" /> instance built at startup
/// </summary>
public class DomainAccessor
{
    /// <summary>
    ///     Create an instance
    /// </summary>
    public DomainAccessor(Domain domain)
    {
        Domain = domain;
    }

    /// <summary>
    ///     The domain built at startup
    /// </summary>
    public Domain Domain { get; }
}
