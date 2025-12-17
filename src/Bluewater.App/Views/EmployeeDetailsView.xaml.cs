using System.Collections;
using System.Windows.Input;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.Core.DepartmentAggregate;

namespace Bluewater.App.Views;

public partial class EmployeeDetailsView : ContentView
{
  private readonly IReferenceDataService referenceDataService;
  private bool isSelectionUpdateInProgress;

  public EmployeeDetailsView()
  {
    referenceDataService = ResolveReferenceDataService();

    InitializeComponent();

    UpdateEmptyState();
    UpdatePickerSelections();
  }

  public IReadOnlyList<PositionSummary> Positions => referenceDataService.Positions;

  public IReadOnlyList<EmployeeTypeSummary> EmployeeTypes => referenceDataService.EmployeeTypes;

  public IReadOnlyList<LevelSummary> Levels => referenceDataService.Levels;

  public IReadOnlyList<ChargingSummary> Chargings => referenceDataService.Chargings;

  public static readonly BindableProperty TitleProperty = BindableProperty.Create(
          nameof(Title), typeof(string), typeof(EmployeeDetailsView), string.Empty);

  public static readonly BindableProperty IsOpenProperty = BindableProperty.Create(
          nameof(IsOpen), typeof(bool), typeof(EmployeeDetailsView), false);

  public static readonly BindableProperty EditableEmployeeProperty = BindableProperty.Create(
          nameof(EditableEmployee), typeof(EditableEmployee), typeof(EmployeeDetailsView),
          propertyChanged: OnEditableEmployeeChanged);

  public static readonly BindableProperty UpdateCommandProperty = BindableProperty.Create(
          nameof(UpdateCommand), typeof(ICommand), typeof(EmployeeDetailsView));

  public static readonly BindableProperty UpdateCommandParameterProperty = BindableProperty.Create(
          nameof(UpdateCommandParameter), typeof(object), typeof(EmployeeDetailsView));

  public static readonly BindableProperty PrimaryButtonTextProperty = BindableProperty.Create(
          nameof(PrimaryButtonText), typeof(string), typeof(EmployeeDetailsView), "Save Employee");

  public static readonly BindableProperty IsUpdateEnabledProperty = BindableProperty.Create(
          nameof(IsUpdateEnabled), typeof(bool), typeof(EmployeeDetailsView), true,
          propertyChanged: OnIsUpdateEnabledChanged);

  public static readonly BindableProperty CloseCommandProperty = BindableProperty.Create(
          nameof(CloseCommand), typeof(ICommand), typeof(EmployeeDetailsView));

  public static readonly BindableProperty CloseCommandParameterProperty = BindableProperty.Create(
          nameof(CloseCommandParameter), typeof(object), typeof(EmployeeDetailsView));

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

  public EditableEmployee? EditableEmployee
  {
    get => (EditableEmployee?)GetValue(EditableEmployeeProperty);
    set => SetValue(EditableEmployeeProperty, value);
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

  public string PrimaryButtonText
  {
    get => (string)GetValue(PrimaryButtonTextProperty);
    set => SetValue(PrimaryButtonTextProperty, value);
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

  private static void OnEditableEmployeeChanged(BindableObject bindable, object oldValue, object newValue)
  {
    if (bindable is not EmployeeDetailsView view)
    {
      return;
    }

    view.FormLayout.BindingContext = newValue as EditableEmployee;
    view.UpdatePickerSelections();
    view.UpdateEmptyState();
  }

  private static void OnIsUpdateEnabledChanged(BindableObject bindable, object oldValue, object newValue)
  {
    if (bindable is not EmployeeDetailsView view)
    {
      return;
    }

    view.UpdateEmptyState();
  }

  private void UpdateEmptyState()
  {
    bool hasEmployee = EditableEmployee is not null;

    EmptyStateLayout.IsVisible = !hasEmployee;
    FormScrollView.IsVisible = hasEmployee;
    EditorView.IsUpdateEnabled = hasEmployee && IsUpdateEnabled;
  }

  private void UpdatePickerSelections()
  {
    if (PositionPicker is null || TypePicker is null || LevelPicker is null || ChargingPicker is null)
    {
      return;
    }

    isSelectionUpdateInProgress = true;

    try
    {
      if (EditableEmployee is null)
      {
        ClearPickerSelection(PositionPicker);
        ClearPickerSelection(TypePicker);
        ClearPickerSelection(LevelPicker);
        ClearPickerSelection(ChargingPicker);
        return;
      }

      SetPickerSelection(PositionPicker,
        Positions.FirstOrDefault(position => position.Id == EditableEmployee.PositionId)
          ?? (!string.IsNullOrWhiteSpace(EditableEmployee.Position)
              ? Positions.FirstOrDefault(position => string.Equals(position.Name, EditableEmployee.Position, StringComparison.OrdinalIgnoreCase))
              : null));

      SetPickerSelection(TypePicker,
        EmployeeTypes.FirstOrDefault(type => type.Id == EditableEmployee.TypeId)
          ?? (!string.IsNullOrWhiteSpace(EditableEmployee.Type)
              ? EmployeeTypes.FirstOrDefault(type => string.Equals(type.Name, EditableEmployee.Type, StringComparison.OrdinalIgnoreCase))
              : null));

      SetPickerSelection(LevelPicker,
        Levels.FirstOrDefault(level => level.Id == EditableEmployee.LevelId)
          ?? (!string.IsNullOrWhiteSpace(EditableEmployee.Level)
              ? Levels.FirstOrDefault(level => string.Equals(level.Name, EditableEmployee.Level, StringComparison.OrdinalIgnoreCase))
              : null));

      SetPickerSelection(ChargingPicker,
        Chargings.FirstOrDefault(charging => charging.Id == EditableEmployee.ChargingId));
    }
    finally
    {
      isSelectionUpdateInProgress = false;
    }
  }

  private static void ClearPickerSelection(Picker picker)
  {
    if (picker.ItemsSource is IList)
    {
      picker.SelectedIndex = -1;
    }

    picker.SelectedItem = null;
  }

  private static void SetPickerSelection<T>(Picker picker, T? item)
    where T : class
  {
    if (picker.ItemsSource is not IList items)
    {
      picker.SelectedItem = item;
      picker.SelectedIndex = item is null ? -1 : picker.SelectedIndex;
      return;
    }

    if (item is null)
    {
      picker.SelectedIndex = -1;
      picker.SelectedItem = null;
      return;
    }

    int index = -1;

    for (int i = 0; i < items.Count; i++)
    {
      if (items[i] is T candidate && EqualityComparer<T>.Default.Equals(candidate, item))
      {
        index = i;
        break;
      }
    }

    picker.SelectedIndex = index;
    picker.SelectedItem = index >= 0 ? items[index] : null;
  }

  private void OnPositionPickerSelectedIndexChanged(object? sender, EventArgs e)
  {
    if (isSelectionUpdateInProgress || EditableEmployee is null)
    {
      return;
    }

    if (PositionPicker.SelectedItem is PositionSummary selectedPosition)
    {
      EditableEmployee.PositionId = selectedPosition.Id;
      EditableEmployee.Position = selectedPosition.Name;

      // check for section where this position id belongs. 
      var _position = referenceDataService.Positions.FirstOrDefault(i => i.Id.Equals(selectedPosition.Id));
      if (_position != null) { 
        var _section = referenceDataService.Sections.FirstOrDefault(i => i.Id.Equals(_position.SectionId));
        if (_section != null)
        {
          EditableEmployee.Section = _section.Name;

          var _department = referenceDataService.Departments.FirstOrDefault(i => i.Id.Equals(_section.DepartmentId));
          if (_department != null)
            EditableEmployee.Department = _department.Name;
          else
            EditableEmployee.Department = null;
        }
      }
    }
    else
    {
      EditableEmployee.PositionId = null;
      EditableEmployee.Position = null;
    }
  }

  private void OnTypePickerSelectedIndexChanged(object? sender, EventArgs e)
  {
    if (isSelectionUpdateInProgress || EditableEmployee is null)
    {
      return;
    }

    if (TypePicker.SelectedItem is EmployeeTypeSummary selectedType)
    {
      EditableEmployee.TypeId = selectedType.Id;
      EditableEmployee.Type = selectedType.Name;
    }
    else
    {
      EditableEmployee.TypeId = null;
      EditableEmployee.Type = null;
    }
  }

  private void OnLevelPickerSelectedIndexChanged(object? sender, EventArgs e)
  {
    if (isSelectionUpdateInProgress || EditableEmployee is null)
    {
      return;
    }

    if (LevelPicker.SelectedItem is LevelSummary selectedLevel)
    {
      EditableEmployee.LevelId = selectedLevel.Id;
      EditableEmployee.Level = selectedLevel.Name;
    }
    else
    {
      EditableEmployee.LevelId = null;
      EditableEmployee.Level = null;
    }
  }

  private void OnChargingPickerSelectedIndexChanged(object? sender, EventArgs e)
  {
    if (isSelectionUpdateInProgress || EditableEmployee is null)
    {
      return;
    }

    if (ChargingPicker.SelectedItem is ChargingSummary selectedCharging)
    {
      EditableEmployee.ChargingId = selectedCharging.Id;
    }
    else
    {
      EditableEmployee.ChargingId = null;
    }
  }

  private static IReferenceDataService ResolveReferenceDataService()
  {
    if (Application.Current is App app)
    {
      return app.Services.GetRequiredService<IReferenceDataService>();
    }

    throw new InvalidOperationException("Unable to resolve reference data service.");
  }
}
