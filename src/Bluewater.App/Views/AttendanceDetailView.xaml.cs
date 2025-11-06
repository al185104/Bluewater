using System.Collections.Generic;
using System.Windows.Input;
using Bluewater.App.Models;
using Microsoft.Maui.Controls;

namespace Bluewater.App.Views;

public partial class AttendanceDetailView : ContentView
{
  public AttendanceDetailView()
  {
    InitializeComponent();
  }

  public static readonly BindableProperty TitleProperty = BindableProperty.Create(
    nameof(Title), typeof(string), typeof(AttendanceDetailView), string.Empty);

  public static readonly BindableProperty IsOpenProperty = BindableProperty.Create(
    nameof(IsOpen), typeof(bool), typeof(AttendanceDetailView), false);

  public static readonly BindableProperty SummaryProperty = BindableProperty.Create(
    nameof(Summary), typeof(EmployeeAttendanceSummary), typeof(AttendanceDetailView), null);

  public static readonly BindableProperty AttendancesProperty = BindableProperty.Create(
    nameof(Attendances), typeof(IEnumerable<AttendanceSummary>), typeof(AttendanceDetailView), null);

  public static readonly BindableProperty UpdateCommandProperty = BindableProperty.Create(
    nameof(UpdateCommand), typeof(ICommand), typeof(AttendanceDetailView));

  public static readonly BindableProperty UpdateCommandParameterProperty = BindableProperty.Create(
    nameof(UpdateCommandParameter), typeof(object), typeof(AttendanceDetailView));

  public static readonly BindableProperty UpdateButtonTextProperty = BindableProperty.Create(
    nameof(UpdateButtonText), typeof(string), typeof(AttendanceDetailView), "Close");

  public static readonly BindableProperty IsUpdateEnabledProperty = BindableProperty.Create(
    nameof(IsUpdateEnabled), typeof(bool), typeof(AttendanceDetailView), true);

  public static readonly BindableProperty CloseCommandProperty = BindableProperty.Create(
    nameof(CloseCommand), typeof(ICommand), typeof(AttendanceDetailView));

  public static readonly BindableProperty CloseCommandParameterProperty = BindableProperty.Create(
    nameof(CloseCommandParameter), typeof(object), typeof(AttendanceDetailView));

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

  public EmployeeAttendanceSummary? Summary
  {
    get => (EmployeeAttendanceSummary?)GetValue(SummaryProperty);
    set => SetValue(SummaryProperty, value);
  }

  public IEnumerable<AttendanceSummary>? Attendances
  {
    get => (IEnumerable<AttendanceSummary>?)GetValue(AttendancesProperty);
    set => SetValue(AttendancesProperty, value);
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

  public string UpdateButtonText
  {
    get => (string)GetValue(UpdateButtonTextProperty);
    set => SetValue(UpdateButtonTextProperty, value);
  }

  public bool IsUpdateEnabled
  {
    get => (bool)GetValue(IsUpdateEnabledProperty);
    set => SetValue(IsUpdateEnabledProperty, value);
  }

  public ICommand? CloseCommand
  {
    get => (ICommand?)GetValue(CloseCommandProperty);
    set => SetValue(CloseCommandProperty, value);
  }

  public object? CloseCommandParameter
  {
    get => GetValue(CloseCommandParameterProperty);
    set => SetValue(CloseCommandParameterProperty, value);
  }
}
