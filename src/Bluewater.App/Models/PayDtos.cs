using System;

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

public class CreatePayResponseDto
{
  public CreatePayRecordDto? Pay { get; set; }
}

public class CreatePayRecordDto
{
  public Guid Id { get; set; }
}
