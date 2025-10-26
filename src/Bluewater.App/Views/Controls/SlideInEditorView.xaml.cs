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

		public static readonly BindableProperty CloseCommandProperty = BindableProperty.Create(
			nameof(CloseCommand), typeof(ICommand), typeof(SlideInEditorView));

		public static readonly BindableProperty CloseCommandParameterProperty = BindableProperty.Create(
			nameof(CloseCommandParameter), typeof(object), typeof(SlideInEditorView));

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

				isAnimating = true;

				try
				{
						await EnsurePanelWidthAsync();

						double targetWidth = PanelBorder.Width > 0 ? PanelBorder.Width : PanelBorder.WidthRequest;

						if (isOpen)
						{
								OverlayLayer.IsVisible = true;
								OverlayLayer.InputTransparent = false;
								PanelBorder.IsVisible = true;
								PanelBorder.TranslationX = targetWidth;
								await Task.WhenAll(
									OverlayLayer.FadeTo(1, AnimationDuration, Easing.SinOut),
									PanelBorder.TranslateTo(0, 0, AnimationDuration, Easing.SinOut));
								InputTransparent = false;
						}
						else
						{
								await Task.WhenAll(
									PanelBorder.TranslateTo(targetWidth, 0, AnimationDuration, Easing.SinIn),
									OverlayLayer.FadeTo(0, AnimationDuration, Easing.SinIn));
								PanelBorder.IsVisible = false;
								OverlayLayer.InputTransparent = true;
								OverlayLayer.IsVisible = false;
								InputTransparent = true;
						}
				}
				finally
				{
						isAnimating = false;
				}
		}

		private async Task EnsurePanelWidthAsync()
		{
				if (PanelBorder.Width > 0 || PanelBorder.WidthRequest > 0)
				{
						InitializeTranslationIfNeeded(PanelBorder.Width > 0 ? PanelBorder.Width : PanelBorder.WidthRequest);
						return;
				}

				if (Width > 0)
				{
						double initialWidth = Math.Max(0, Width / 2);
						PanelBorder.WidthRequest = initialWidth;
						InitializeTranslationIfNeeded(initialWidth);
						return;
				}

				var completionSource = new TaskCompletionSource();

				void Handler(object? sender, EventArgs args)
				{
						if (Width <= 0)
						{
								return;
						}

						SizeChanged -= Handler;
						completionSource.TrySetResult();
				}

				SizeChanged += Handler;

				if (Width > 0)
				{
						SizeChanged -= Handler;
				}
				else
				{
						await completionSource.Task;
				}

				double targetWidth = Math.Max(0, Width / 2);
				PanelBorder.WidthRequest = targetWidth;
				InitializeTranslationIfNeeded(targetWidth);
		}

		private void InitializeTranslationIfNeeded(double panelWidth)
		{
				if (hasInitializedTranslation || panelWidth <= 0)
				{
						return;
				}

				PanelBorder.TranslationX = panelWidth;
				hasInitializedTranslation = true;
		}

		protected override void OnHandlerChanged()
		{
				if (Handler is null)
				{
						SizeChanged -= OnSizeChanged;
				}
				else
				{
						SizeChanged -= OnSizeChanged;
						SizeChanged += OnSizeChanged;
				}

				base.OnHandlerChanged();
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

		private void OnOverlayTapped(object? sender, TappedEventArgs e)
		{
				Close();
		}

		private void OnCloseButtonClicked(object? sender, EventArgs e)
		{
				Close();
		}

		private void Close()
		{
				if (CloseCommand?.CanExecute(CloseCommandParameter) == true)
				{
						CloseCommand.Execute(CloseCommandParameter);
						return;
				}

				if (IsOpen)
				{
						IsOpen = false;
				}
		}
}
