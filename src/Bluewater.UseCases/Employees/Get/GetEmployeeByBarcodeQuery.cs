using Ardalis.Result;

namespace Bluewater.UseCases.Employees.Get;

public record GetEmployeeByBarcodeQuery(string barcode) : Ardalis.SharedKernel.IQuery<Result<EmployeeShortDTO>>;