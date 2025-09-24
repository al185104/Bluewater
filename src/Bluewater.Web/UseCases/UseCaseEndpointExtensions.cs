using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using OpenApiEndpointConventionBuilderExtensions = Microsoft.AspNetCore.OpenApi.OpenApiEndpointConventionBuilderExtensions;
using OpenApiEndpointRouteBuilderExtensions = Microsoft.AspNetCore.OpenApi.OpenApiEndpointRouteBuilderExtensions;

namespace Bluewater.Web.UseCases;

internal static class UseCaseEndpointExtensions
{
  private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
  {
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
  };

  static UseCaseEndpointExtensions()
  {
    SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true));
  }

  public static void MapUseCaseEndpoints(this WebApplication app)
  {
    var useCaseAssembly = typeof(Bluewater.UseCases.Contributors.Create.CreateContributorCommand).Assembly;
    var descriptors = UseCaseDescriptor.Discover(useCaseAssembly);

    if (descriptors.Count == 0)
    {
      return;
    }

    var group = app.MapGroup("/usecases");
    group.WithTags("UseCases");
    OpenApiEndpointRouteBuilderExtensions.WithOpenApi(group);

    var listBuilder = group.MapGet("/", () => descriptors.Select(d => new
    {
      d.Kind,
      Category = d.CategorySegments,
      Operation = d.OperationName,
      Route = $"/usecases/{d.Route}",
      RequestType = d.RequestType.FullName,
      ResultType = d.ResultType.FullName,
      ResultValueType = d.ResultValueType?.FullName
    }))
    .WithName("UseCases.List");

    OpenApiEndpointConventionBuilderExtensions.WithOpenApi(listBuilder, op =>
    {
      op.Summary = "List available Bluewater use case endpoints.";
      return op;
    });

    foreach (var descriptor in descriptors)
    {
      var localDescriptor = descriptor;
      var methods = localDescriptor.IsCommand ? new[] { "POST" } : new[] { "GET", "POST" };

      Delegate handler = new Func<HttpContext, IMediator, CancellationToken, Task<IResult>>(async (context, mediator, ct) =>
      {
        var requestObject = await BindRequestAsync(context, localDescriptor.RequestType, ct);

        if (requestObject is null)
        {
          return Results.BadRequest(new
          {
            error = $"A payload or query string is required to execute {localDescriptor.RequestType.Name}."
          });
        }

        var response = await mediator.Send(requestObject, ct);
        return ConvertResult(response, localDescriptor);
      });

      var builder = group.MapMethods($"/{localDescriptor.Route}", methods, handler);

      builder.WithName($"UseCases.{localDescriptor.DisplayName}");

      OpenApiEndpointConventionBuilderExtensions.WithOpenApi(builder, op =>
      {
        op.Summary = $"Execute {localDescriptor.DisplayName}.";
        op.OperationId ??= $"UseCases_{localDescriptor.DisplayName.Replace('.', '_')}";
        op.Tags ??= new List<OpenApiTag>();
        if (localDescriptor.CategorySegments.Length > 0)
        {
          op.Tags.Add(new OpenApiTag { Name = string.Join(" / ", localDescriptor.CategorySegments) });
        }
        else
        {
          op.Tags.Add(new OpenApiTag { Name = "UseCases" });
        }
        return op;
      });
    }
  }

  private static async Task<object?> BindRequestAsync(HttpContext context, Type requestType, CancellationToken cancellationToken)
  {
    if (context.Request.ContentLength.HasValue && context.Request.ContentLength.Value > 0)
    {
      return await JsonSerializer.DeserializeAsync(context.Request.Body, requestType, SerializerOptions, cancellationToken);
    }

    if (context.Request.Query.Count > 0)
    {
      var queryDictionary = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

      foreach (var (key, value) in context.Request.Query)
      {
        if (value.Count > 1)
        {
          queryDictionary[key] = value.ToArray();
        }
        else
        {
          queryDictionary[key] = value.ToString();
        }
      }

      var json = JsonSerializer.Serialize(queryDictionary, SerializerOptions);
      return JsonSerializer.Deserialize(json, requestType, SerializerOptions);
    }

    return null;
  }

  private static IResult ConvertResult(object? response, UseCaseDescriptor descriptor)
  {
    if (response is null)
    {
      return Results.Ok();
    }

    if (response is IResult httpResult)
    {
      return httpResult;
    }

    if (response is Result nonGenericResult)
    {
      return MapStatus(nonGenericResult.Status, null, nonGenericResult.Errors, nonGenericResult.ValidationErrors, descriptor);
    }

    var responseType = response.GetType();

    if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
    {
      var status = (ResultStatus)responseType.GetProperty(nameof(Result.Status))!.GetValue(response)!;
      var errors = (IEnumerable<string>)responseType.GetProperty(nameof(Result.Errors))!.GetValue(response)!;
      var validationErrors = responseType.GetProperty(nameof(Result.ValidationErrors))!.GetValue(response);
      var value = responseType.GetProperty("Value")?.GetValue(response);

      return MapStatus(status, value, errors, validationErrors, descriptor);
    }

    return Results.Ok(response);
  }

  private static IResult MapStatus(ResultStatus status, object? value, IEnumerable<string>? errors, object? validationErrors, UseCaseDescriptor descriptor)
  {
    return status switch
    {
      ResultStatus.Ok => value is null ? Results.Ok() : Results.Ok(value),
      ResultStatus.Created => Results.Created($"/usecases/{descriptor.Route}", value),
      ResultStatus.NoContent => Results.NoContent(),
      ResultStatus.Invalid => Results.BadRequest(new { errors, validationErrors }),
      ResultStatus.NotFound => Results.NotFound(new { errors }),
      ResultStatus.Error => Results.Problem(string.Join(Environment.NewLine, errors ?? Array.Empty<string>())),
      ResultStatus.CriticalError => Results.Problem(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()), statusCode: StatusCodes.Status500InternalServerError),
      ResultStatus.Unauthorized => Results.Unauthorized(),
      ResultStatus.Forbidden => Results.StatusCode(StatusCodes.Status403Forbidden),
      ResultStatus.Conflict => Results.Conflict(new { errors }),
      _ => value is null ? Results.Ok() : Results.Ok(value)
    };
  }
}
