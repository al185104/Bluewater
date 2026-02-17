namespace Bluewater.App.Components;

public partial class LabeledEntryView : ContentView
{
		public LabeledEntryView()
		{
				InitializeComponent();
		}

		public static readonly BindableProperty TitleProperty =
				BindableProperty.Create(nameof(Title), typeof(string), typeof(LabeledEntryView), string.Empty);

		public static readonly BindableProperty TextProperty =
				BindableProperty.Create(nameof(Text), typeof(string), typeof(LabeledEntryView), string.Empty, BindingMode.TwoWay);

		public static readonly BindableProperty PlaceholderProperty =
				BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(LabeledEntryView), string.Empty);

		public static readonly BindableProperty KeyboardProperty =
				BindableProperty.Create(nameof(Keyboard), typeof(Keyboard), typeof(LabeledEntryView), Keyboard.Default);

		public static readonly BindableProperty IsPasswordProperty =
				BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(LabeledEntryView), false);

		public static readonly BindableProperty TitleColorProperty =
				BindableProperty.Create(nameof(TitleColor), typeof(Color), typeof(LabeledEntryView), Colors.Gray);

		public static readonly BindableProperty TextColorProperty =
				BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(LabeledEntryView), Colors.Black);

		public string Title
		{
				get => (string)GetValue(TitleProperty);
				set => SetValue(TitleProperty, value);
		}

		public string Text
		{
				get => (string)GetValue(TextProperty);
				set => SetValue(TextProperty, value);
		}

		public string Placeholder
		{
				get => (string)GetValue(PlaceholderProperty);
				set => SetValue(PlaceholderProperty, value);
		}

		public Keyboard Keyboard
		{
				get => (Keyboard)GetValue(KeyboardProperty);
				set => SetValue(KeyboardProperty, value);
		}

		public bool IsPassword
		{
				get => (bool)GetValue(IsPasswordProperty);
				set => SetValue(IsPasswordProperty, value);
		}

		public Color TitleColor
		{
				get => (Color)GetValue(TitleColorProperty);
				set => SetValue(TitleColorProperty, value);
		}

		public Color TextColor
		{
				get => (Color)GetValue(TextColorProperty);
				set => SetValue(TextColorProperty, value);
		}
}
