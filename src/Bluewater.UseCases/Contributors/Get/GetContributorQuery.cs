using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Contributors.Get;
public record GetContributorQuery(int ContributorId) : IQuery<Result<ContributorDTO>>;
