namespace Bluewater.App.Models;

public record ActivityTraceEntry(
  DateTimeOffset Timestamp,
  string EventType,
  string? FromRoute = null,
  string? ToRoute = null,
  string? CommandName = null,
  object? Metadata = null);
