using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Microsoft.Maui.Storage;

namespace Bluewater.App.Services;

public sealed class ActivityTraceService : IActivityTraceService, IAsyncDisposable
{
		private const string LogFilePrefix = "activity-trace-";
		private readonly SemaphoreSlim writeLock = new(1, 1);
		private readonly JsonSerializerOptions serializerOptions = new()
		{
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};

		private readonly string appDataDirectory;

		public ActivityTraceService(string? appDataDirectory = null)
		{
				this.appDataDirectory = ResolveWritableLogDirectory(appDataDirectory);
		}

		public Task<string> GetCurrentLogPathAsync()
		{
				string logFilePath = GetLogFilePath(DateTimeOffset.UtcNow);
				return Task.FromResult(logFilePath);
		}

		public Task LogCommandAsync(string commandName, object? metadata = null)
		{
				if (string.IsNullOrWhiteSpace(commandName))
				{
						throw new ArgumentException("Command name must be provided.", nameof(commandName));
				}

				var entry = new ActivityTraceEntry(
					Timestamp: DateTimeOffset.UtcNow,
					EventType: "Command",
					CommandName: commandName,
					Metadata: metadata);

				return AppendEntryAsync(entry);
		}

		public Task LogNavigationAsync(string? fromRoute, string? toRoute, object? metadata = null)
		{
				var entry = new ActivityTraceEntry(
					Timestamp: DateTimeOffset.UtcNow,
					EventType: "Navigation",
					FromRoute: fromRoute,
					ToRoute: toRoute,
					Metadata: metadata);

				return AppendEntryAsync(entry);
		}

		private async Task AppendEntryAsync(ActivityTraceEntry entry)
		{
				string logFilePath = GetLogFilePath(entry.Timestamp);
				Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)!);

				string serializedEntry = JsonSerializer.Serialize(entry, serializerOptions);

				await writeLock.WaitAsync().ConfigureAwait(false);
				try
				{
						await using var stream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
						await using var writer = new StreamWriter(stream, Encoding.UTF8);
						await writer.WriteLineAsync(serializedEntry).ConfigureAwait(false);
				}
				finally
				{
						writeLock.Release();
				}
		}

		private string GetLogFilePath(DateTimeOffset timestamp)
		{
				string fileName = $"{LogFilePrefix}{timestamp:yyyyMMdd}.log";
				return Path.Combine(appDataDirectory, fileName);
		}

		private static string GetDefaultLogDirectory()
		{
				string mauiAppDataPath = FileSystem.Current.AppDataDirectory;
				if (!string.IsNullOrWhiteSpace(mauiAppDataPath))
				{
						return Path.Combine(mauiAppDataPath, "Logs");
				}

				string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
				if (!string.IsNullOrWhiteSpace(localAppDataPath))
				{
						return Path.Combine(localAppDataPath, "Bluewater", "App", "Logs");
				}

				return Path.Combine(Path.GetTempPath(), "Bluewater", "App", "Logs");
		}

		private static string ResolveWritableLogDirectory(string? preferredDirectory)
		{
				if (!string.IsNullOrWhiteSpace(preferredDirectory))
				{
						Directory.CreateDirectory(preferredDirectory);
						return preferredDirectory;
				}

				string[] candidates =
				[
						GetDefaultLogDirectory(),
						Path.Combine(Path.GetTempPath(), "Bluewater", "App", "Logs")
				];

				foreach (string candidate in candidates)
				{
						try
						{
								Directory.CreateDirectory(candidate);
								return candidate;
						}
						catch (UnauthorizedAccessException)
						{
								// Move to next fallback.
						}
						catch (IOException)
						{
								// Move to next fallback.
						}
				}

				throw new InvalidOperationException("Unable to initialize a writable directory for activity trace logs.");
		}

		public ValueTask DisposeAsync()
		{
				writeLock.Dispose();
				return ValueTask.CompletedTask;
		}
}
