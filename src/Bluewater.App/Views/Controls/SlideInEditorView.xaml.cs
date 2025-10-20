using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace Bluewater.App.Views.Controls;

public partial class SlideInEditorView : ContentView
{
  private const uint AnimationDuration = 250;
  private bool hasInitializedTranslation;
  private bool isAnimating;

  public SlideInEditorView()
  {
    InitializeComponent();
    SizeChanged += OnSizeChanged;
    //IsHitTestVisible = false;
  }

  public static readonly BindableProperty TitleProperty = BindableProperty.Create(
    nameof(Title), typeof(string), typeof(SlideInEditorView), string.Empty);

  public static readonly BindableProperty IsOpenProperty = BindableProperty.Create(
    nameof(IsOpen), typeof(bool), typeof(SlideInEditorView), false,
    propertyChanged: OnIsOpenChanged);

  public static readonly BindableProperty EditorContentProperty = BindableProperty.Create(
    nameof(EditorContent), typeof(View), typeof(SlideInEditorView));

  public static readonly BindableProperty UpdateCommandProperty = BindableProperty.Create(
    nameof(UpdateCommand), typeof(ICommand), typeof(SlideInEditorView));

  public static readonly BindableProperty UpdateCommandParameterProperty = BindableProperty.Create(
    nameof(UpdateCommandParameter), typeof(object), typeof(SlideInEditorView));

  public static readonly BindableProperty IsUpdateEnabledProperty = BindableProperty.Create(
    nameof(IsUpdateEnabled), typeof(bool), typeof(SlideInEditorView), true);

  public static readonly BindableProperty UpdateButtonTextProperty = BindableProperty.Create(
    nameof(UpdateButtonText), typeof(string), typeof(SlideInEditorView), "Update");

  public string Title
  {
    get => (string)GetValue(TitleProperty);
    set => SetValue(TitleProperty, value);
  }

  public bool IsOpen
  {
    get => (bool)GetValue(IsOpenProperty);
    set => SetValue(IsOpenProperty, value);
  }

  public View? EditorContent
  {
    get => (View?)GetValue(EditorContentProperty);
    set => SetValue(EditorContentProperty, value);
  }

  public ICommand? UpdateCommand
  {
    get => (ICommand?)GetValue(UpdateCommandProperty);
    set => SetValue(UpdateCommandProperty, value);
  }

  public object? UpdateCommandParameter
  {
    get => GetValue(UpdateCommandParameterProperty);
    set => SetValue(UpdateCommandParameterProperty, value);
  }

  public bool IsUpdateEnabled
  {
    get => (bool)GetValue(IsUpdateEnabledProperty);
    set => SetValue(IsUpdateEnabledProperty, value);
  }

  public string UpdateButtonText
  {
    get => (string)GetValue(UpdateButtonTextProperty);
    set => SetValue(UpdateButtonTextProperty, value);
  }

  private static void OnIsOpenChanged(BindableObject bindable, object oldValue, object newValue)
  {
    if (bindable is SlideInEditorView view && newValue is bool isOpen)
    {
      _ = view.ToggleAsync(isOpen);
    }
  }

  private async Task ToggleAsync(bool isOpen)
  {
    if (isAnimating)
    {
      return;
    }

    if (PanelBorder.Width <= 0)
    {
      PanelBorder.WidthRequest = Math.Max(0, Width / 2);
    }

    if (!hasInitializedTranslation)
    {
      PanelBorder.TranslationX = PanelBorder.WidthRequest;
      hasInitializedTranslation = true;
    }

    isAnimating = true;

    if (isOpen)
    {
      //IsHitTestVisible = true;
      PanelBorder.IsVisible = true;
      double targetWidth = PanelBorder.Width > 0 ? PanelBorder.Width : PanelBorder.WidthRequest;
      PanelBorder.TranslationX = targetWidth;
      await PanelBorder.TranslateTo(0, 0, AnimationDuration, Easing.SinOut);
    }
    else
    {
      double targetWidth = PanelBorder.Width > 0 ? PanelBorder.Width : PanelBorder.WidthRequest;
      await PanelBorder.TranslateTo(targetWidth, 0, AnimationDuration, Easing.SinIn);
      PanelBorder.IsVisible = false;
      //IsHitTestVisible = false;
    }

    isAnimating = false;
  }

  private void OnSizeChanged(object? sender, EventArgs e)
  {
    double targetWidth = Width / 2;

    if (targetWidth <= 0)
    {
      return;
    }

    PanelBorder.WidthRequest = targetWidth;

    if (!IsOpen)
    {
      PanelBorder.TranslationX = targetWidth;
    }
  }
}
