using System;

namespace Bluewater.App.Interfaces;

public interface IExceptionHandlingService
{
  void Initialize();

  void Handle(Exception exception, string context);
}
