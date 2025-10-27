using System;
using System.Collections.Generic;
using System.Windows.Input;
using Bluewater.App.Models;
using Microsoft.Maui.Controls;

namespace Bluewater.App.Views;

public partial class EmployeeDetailsView : ContentView
{
		public EmployeeDetailsView()
		{
				InitializeComponent();
				UpdateEmptyState();
		}

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
                public static readonly BindableProperty UserOptionsProperty = BindableProperty.Create(
                        nameof(UserOptions), typeof(IEnumerable<EmployeeExternalOption>), typeof(EmployeeDetailsView), Array.Empty<EmployeeExternalOption>());

                public static readonly BindableProperty PositionOptionsProperty = BindableProperty.Create(
                        nameof(PositionOptions), typeof(IEnumerable<EmployeeExternalOption>), typeof(EmployeeDetailsView), Array.Empty<EmployeeExternalOption>());

                public static readonly BindableProperty PayOptionsProperty = BindableProperty.Create(
                        nameof(PayOptions), typeof(IEnumerable<EmployeeExternalOption>), typeof(EmployeeDetailsView), Array.Empty<EmployeeExternalOption>());

                public static readonly BindableProperty TypeOptionsProperty = BindableProperty.Create(
                        nameof(TypeOptions), typeof(IEnumerable<EmployeeExternalOption>), typeof(EmployeeDetailsView), Array.Empty<EmployeeExternalOption>());

                public static readonly BindableProperty LevelOptionsProperty = BindableProperty.Create(
                        nameof(LevelOptions), typeof(IEnumerable<EmployeeExternalOption>), typeof(EmployeeDetailsView), Array.Empty<EmployeeExternalOption>());

                public static readonly BindableProperty ChargingOptionsProperty = BindableProperty.Create(
                        nameof(ChargingOptions), typeof(IEnumerable<EmployeeExternalOption>), typeof(EmployeeDetailsView), Array.Empty<EmployeeExternalOption>());


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

                public IEnumerable<EmployeeExternalOption> UserOptions
                {
                                get => (IEnumerable<EmployeeExternalOption>)GetValue(UserOptionsProperty);
                                set => SetValue(UserOptionsProperty, value);
                }

                public IEnumerable<EmployeeExternalOption> PositionOptions
                {
                                get => (IEnumerable<EmployeeExternalOption>)GetValue(PositionOptionsProperty);
                                set => SetValue(PositionOptionsProperty, value);
                }

                public IEnumerable<EmployeeExternalOption> PayOptions
                {
                                get => (IEnumerable<EmployeeExternalOption>)GetValue(PayOptionsProperty);
                                set => SetValue(PayOptionsProperty, value);
                }

                public IEnumerable<EmployeeExternalOption> TypeOptions
                {
                                get => (IEnumerable<EmployeeExternalOption>)GetValue(TypeOptionsProperty);
                                set => SetValue(TypeOptionsProperty, value);
                }

                public IEnumerable<EmployeeExternalOption> LevelOptions
                {
                                get => (IEnumerable<EmployeeExternalOption>)GetValue(LevelOptionsProperty);
                                set => SetValue(LevelOptionsProperty, value);
                }

                public IEnumerable<EmployeeExternalOption> ChargingOptions
                {
                                get => (IEnumerable<EmployeeExternalOption>)GetValue(ChargingOptionsProperty);
                                set => SetValue(ChargingOptionsProperty, value);
                }


		private static void OnEditableEmployeeChanged(BindableObject bindable, object oldValue, object newValue)
		{
				if (bindable is not EmployeeDetailsView view)
				{
						return;
				}

				view.FormLayout.BindingContext = newValue as EditableEmployee;
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
}
