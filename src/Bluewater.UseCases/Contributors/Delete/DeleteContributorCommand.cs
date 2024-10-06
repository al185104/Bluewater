using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Contributors.Delete;
public record DeleteContributorCommand(int ContributorId) : ICommand<Result>;
