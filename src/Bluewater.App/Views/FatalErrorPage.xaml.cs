using Bluewater.App.Exceptions;

namespace Bluewater.App.Views;

public partial class FatalErrorPage : ContentPage
{
  public FatalErrorPage(PresentationException exception)
  {
    ArgumentNullException.ThrowIfNull(exception);

    InitializeComponent();
    BindingContext = exception;
  }
}
