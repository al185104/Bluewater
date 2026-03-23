using Ardalis.SharedKernel;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace Bluewater.Infrastructure.Data;
// inherit from Ardalis.Specification type
public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T> where T : class, IAggregateRoot
{
  private readonly AppDbContext _dbContext;

  public EfRepository(AppDbContext dbContext) : base(dbContext)
  {
    _dbContext = dbContext;
  }

  public override async Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
    where TId : notnull
  {
    if (id is Guid guidId && TryGetGuidPrimaryKey(out IEntityType? entityType, out IProperty? keyProperty))
    {
      return await GetByGuidKeyAsync(guidId, entityType!, keyProperty!, cancellationToken);
    }

    return await base.GetByIdAsync(id, cancellationToken);
  }

  public override async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
  {
    try
    {
      return await base.UpdateAsync(entity, cancellationToken);
    }
    catch (DbUpdateConcurrencyException) when (TryGetGuidPrimaryKey(out _, out _))
    {
      await _dbContext.NormalizeGuidTextAsync(cancellationToken);
      return await base.UpdateAsync(entity, cancellationToken);
    }
  }

  private bool TryGetGuidPrimaryKey(out IEntityType? entityType, out IProperty? keyProperty)
  {
    entityType = _dbContext.Model.FindEntityType(typeof(T));
    keyProperty = entityType?.FindPrimaryKey()?.Properties.SingleOrDefault();

    return entityType is not null &&
           keyProperty is not null &&
           keyProperty.ClrType == typeof(Guid);
  }

  private Task<T?> GetByGuidKeyAsync(Guid id, IEntityType entityType, IProperty keyProperty, CancellationToken cancellationToken)
  {
    string normalizedId = id.ToString("D").ToUpperInvariant();
    StoreObjectIdentifier tableIdentifier = StoreObjectIdentifier.Table(entityType.GetTableName()!, entityType.GetSchema());
    string keyColumnName = keyProperty.GetColumnName(tableIdentifier)!;
    ISqlGenerationHelper sqlGenerationHelper = _dbContext.GetService<ISqlGenerationHelper>();
    string tableName = sqlGenerationHelper.DelimitIdentifier(entityType.GetTableName()!, entityType.GetSchema());
    string keyColumn = sqlGenerationHelper.DelimitIdentifier(keyColumnName);
    string sql = $"SELECT * FROM {tableName} WHERE UPPER({keyColumn}) = @p0";

    return _dbContext
      .Set<T>()
      .FromSqlRaw(sql, normalizedId)
      .SingleOrDefaultAsync(cancellationToken);
  }
}
