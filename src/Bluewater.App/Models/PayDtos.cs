using System;
using System.Collections.Generic;

namespace Bluewater.App.Models;

public class CreatePayRequestDto
{
  public const string Route = "Pays";

  public decimal BasicPay { get; set; }
  public decimal DailyRate { get; set; }
  public decimal HourlyRate { get; set; }
  public decimal HdmfCon { get; set; }
  public decimal HdmfEr { get; set; }
}

public class PayListResponseDto
{
  public List<PayRecordDto> Pays { get; set; } = new();
}

public class CreatePayResponseDto
{
  public CreatePayRecordDto? Pay { get; set; }
}

public class CreatePayRecordDto
{
  public Guid Id { get; set; }
}

public class UpdatePayRequestDto
{
  public const string Route = "Pays/{PayId}";

  public static string BuildRoute(Guid payId) => Route.Replace("{PayId}", payId.ToString());

  public Guid PayId { get; set; }
  public decimal BasicPay { get; set; }
  public decimal DailyRate { get; set; }
  public decimal HourlyRate { get; set; }
  public decimal HdmfCon { get; set; }
  public decimal HdmfEr { get; set; }
  public decimal Cola { get; set; }
}

public class UpdatePayResponseDto
{
  public PayRecordDto? Pay { get; set; }
}
