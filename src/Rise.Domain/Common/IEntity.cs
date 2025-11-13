using System;

namespace Rise.Domain.Common;

public interface IEntity
{
    public Guid Id { get; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
