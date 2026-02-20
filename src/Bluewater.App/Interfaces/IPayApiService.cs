using System;
using System.Collections.Generic;
using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface IPayApiService
{
  Task<IReadOnlyList<PayRecordDto>> GetPaysAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default);

  Task<PayRecordDto?> GetPayByIdAsync(Guid payId, CancellationToken cancellationToken = default);

  Task<PayRecordDto?> CreatePayAsync(PayRecordDto pay, CancellationToken cancellationToken = default);

  Task<PayRecordDto?> UpdatePayAsync(PayRecordDto pay, CancellationToken cancellationToken = default);

  Task<bool> DeletePayAsync(Guid payId, CancellationToken cancellationToken = default);
}
