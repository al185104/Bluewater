using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Bluewater.Infrastructure.Data;

public static class AppDbContextGuidNormalizationExtensions
{
  public static async Task NormalizeGuidTextAsync(this AppDbContext context, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(context);

    HashSet<string> normalizedColumns = new(StringComparer.Ordinal);

    foreach (var entityType in context.Model.GetEntityTypes())
    {
      StoreObjectIdentifier? tableIdentifier = StoreObjectIdentifier.Create(entityType, StoreObjectType.Table);
      if (!tableIdentifier.HasValue)
      {
        continue;
      }

      foreach (var property in entityType.GetProperties())
      {
        if (property.ClrType != typeof(Guid) && property.ClrType != typeof(Guid?))
        {
          continue;
        }

        string? columnName = property.GetColumnName(tableIdentifier.Value);
        if (string.IsNullOrWhiteSpace(columnName))
        {
          continue;
        }

        string key = $"{tableIdentifier.Value.Schema ?? string.Empty}.{tableIdentifier.Value.Name}.{columnName}";
        if (!normalizedColumns.Add(key))
        {
          continue;
        }

        string sql = BuildUpperCaseUpdateSql(tableIdentifier.Value, columnName);
        await context.Database.ExecuteSqlRawAsync(sql, cancellationToken);
      }
    }
  }

  private static string BuildUpperCaseUpdateSql(StoreObjectIdentifier tableIdentifier, string columnName)
  {
    StringBuilder builder = new();
    builder.Append("UPDATE ");
    AppendDelimitedIdentifier(builder, tableIdentifier.Name, tableIdentifier.Schema);
    builder.Append(" SET ");
    AppendDelimitedIdentifier(builder, columnName);
    builder.Append(" = UPPER(");
    AppendDelimitedIdentifier(builder, columnName);
    builder.Append(") WHERE ");
    AppendDelimitedIdentifier(builder, columnName);
    builder.Append(" IS NOT NULL AND ");
    AppendDelimitedIdentifier(builder, columnName);
    builder.Append(" != UPPER(");
    AppendDelimitedIdentifier(builder, columnName);
    builder.Append(");");

    return builder.ToString();
  }

  private static void AppendDelimitedIdentifier(StringBuilder builder, string name, string? schema = null)
  {
    if (!string.IsNullOrWhiteSpace(schema))
    {
      builder.Append('"');
      builder.Append(schema.Replace("\"", "\"\"", StringComparison.Ordinal));
      builder.Append("\".");
    }

    builder.Append('"');
    builder.Append(name.Replace("\"", "\"\"", StringComparison.Ordinal));
    builder.Append('"');
  }
}
