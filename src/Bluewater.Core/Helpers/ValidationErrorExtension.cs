using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Result;

namespace Bluewater.Core.Helpers;

public static class ValidationErrorExtension
{
  public static ValidationError ToValidationError(this string errorMessage, string identifier = "", string errorCode = "", ValidationSeverity severity = ValidationSeverity.Error)
  {
    return new ValidationError(identifier, errorMessage, errorCode, severity);
  }
}
