using System;

namespace Bluewater.App.Exceptions;

public sealed class PresentationException : Exception
{
  public const string GenericErrorMessage = "Something went wrong. Please try again later or contact your administrator.";

  public PresentationException()
    : base(GenericErrorMessage)
  {
  }

  public PresentationException(string message)
    : base(string.IsNullOrWhiteSpace(message) ? GenericErrorMessage : message)
  {
  }

  public PresentationException(Exception innerException)
    : base(GenericErrorMessage, innerException)
  {
  }

  public PresentationException(string message, Exception innerException)
    : base(string.IsNullOrWhiteSpace(message) ? GenericErrorMessage : message, innerException)
  {
  }
}
