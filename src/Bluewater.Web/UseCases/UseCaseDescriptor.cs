using System.Reflection;
using System.Text;
using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.Web.UseCases;

internal sealed class UseCaseDescriptor
{
  private UseCaseDescriptor(
    string[] categorySegments,
    string operationName,
    string operationSlug,
    string route,
    Type requestType,
    Type resultType,
    Type? resultValueType,
    bool isCommand)
  {
    CategorySegments = categorySegments;
    OperationName = operationName;
    OperationSlug = operationSlug;
    Route = route;
    RequestType = requestType;
    ResultType = resultType;
    ResultValueType = resultValueType;
    IsCommand = isCommand;
  }

  public string[] CategorySegments { get; }

  public string OperationName { get; }

  public string OperationSlug { get; }

  public string Route { get; }

  public Type RequestType { get; }

  public Type ResultType { get; }

  public Type? ResultValueType { get; }

  public bool IsCommand { get; }

  public string Kind => IsCommand ? "Command" : "Query";

  public string DisplayName
  {
    get
    {
      if (CategorySegments.Length == 0)
      {
        return OperationName;
      }

      return string.Join('.', CategorySegments) + "." + OperationName;
    }
  }

  public static IReadOnlyList<UseCaseDescriptor> Discover(Assembly assembly)
  {
    var descriptors = new List<UseCaseDescriptor>();

    foreach (var type in assembly.GetTypes())
    {
      if (TryCreate(type, out var descriptor))
      {
        descriptors.Add(descriptor);
      }
    }

    return descriptors
      .OrderBy(d => string.Join('/', d.CategorySegments))
      .ThenBy(d => d.OperationName, StringComparer.Ordinal)
      .ToArray();
  }

  private static bool TryCreate(Type candidate, out UseCaseDescriptor? descriptor)
  {
    descriptor = null;

    if (!candidate.IsClass || candidate.IsAbstract || candidate.IsGenericTypeDefinition)
    {
      return false;
    }

    if (candidate.Namespace is null || !candidate.Namespace.StartsWith("Bluewater.UseCases", StringComparison.Ordinal))
    {
      return false;
    }

    var commandInterface = candidate
      .GetInterfaces()
      .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));

    var queryInterface = candidate
      .GetInterfaces()
      .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<>));

    var selectedInterface = commandInterface ?? queryInterface;

    if (selectedInterface is null)
    {
      return false;
    }

    var resultType = selectedInterface.GetGenericArguments()[0];

    var isArdalisResult = resultType == typeof(Result) ||
      (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>));

    if (!isArdalisResult)
    {
      return false;
    }

    var namespaceSegments = candidate.Namespace.Split('.', StringSplitOptions.RemoveEmptyEntries);

    if (namespaceSegments.Length < 3)
    {
      return false;
    }

    var categorySegments = namespaceSegments
      .Skip(2)
      .Take(namespaceSegments.Length - 3)
      .ToArray();

    var operationName = TrimSuffix(candidate.Name, "Command", "Query");
    var operationSlug = ToKebabCase(operationName);

    var routeBuilder = new StringBuilder();

    if (categorySegments.Length > 0)
    {
      routeBuilder.Append(string.Join('/', categorySegments.Select(ToKebabCase)));
      routeBuilder.Append('/');
    }

    routeBuilder.Append(operationSlug);

    var route = routeBuilder.ToString();

    descriptor = new UseCaseDescriptor(
      categorySegments,
      operationName,
      operationSlug,
      route,
      candidate,
      resultType,
      resultType.IsGenericType ? resultType.GetGenericArguments()[0] : null,
      selectedInterface == commandInterface);

    return true;
  }

  private static string TrimSuffix(string value, params string[] suffixes)
  {
    foreach (var suffix in suffixes)
    {
      if (value.EndsWith(suffix, StringComparison.Ordinal))
      {
        return value[..^suffix.Length];
      }
    }

    return value;
  }

  private static string ToKebabCase(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      return string.Empty;
    }

    var builder = new StringBuilder(value.Length * 2);

    for (var i = 0; i < value.Length; i++)
    {
      var c = value[i];
      if (char.IsUpper(c))
      {
        if (i > 0 && (char.IsLower(value[i - 1]) || (i + 1 < value.Length && char.IsLower(value[i + 1]))))
        {
          builder.Append('-');
        }

        builder.Append(char.ToLowerInvariant(c));
      }
      else
      {
        builder.Append(c);
      }
    }

    return builder.ToString();
  }
}
