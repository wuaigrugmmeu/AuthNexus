namespace AuthNexus.Domain.Common;

/// <summary>
/// 所有实体的基类，提供基本标识能力
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// 实体唯一标识
    /// </summary>
    public Guid Id { get; protected set; }

    protected Entity()
    {
        Id = Guid.NewGuid();
    }

    protected Entity(Guid id)
    {
        Id = id;
    }
}