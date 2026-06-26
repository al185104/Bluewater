using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Bluewater.App.Models;

public class RangeObservableCollection<T> : ObservableCollection<T>
{
		private bool suppressNotifications;

		public void AddRange(IEnumerable<T> items)
		{
				InsertRange(Count, items);
		}

		public void ReplaceRange(IEnumerable<T> items)
		{
				ArgumentNullException.ThrowIfNull(items);

				List<T> newItems = items as List<T> ?? items.ToList();

				suppressNotifications = true;
				try
				{
						Items.Clear();
						foreach (T item in newItems)
						{
								Items.Add(item);
						}
				}
				finally
				{
						suppressNotifications = false;
				}

				OnPropertyChanged(new(nameof(Count)));
				OnPropertyChanged(new("Item[]"));
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public void InsertRange(int index, IEnumerable<T> items)
		{
				ArgumentNullException.ThrowIfNull(items);

				if (index < 0 || index > Count)
				{
						throw new ArgumentOutOfRangeException(nameof(index));
				}

				List<T> newItems = items as List<T> ?? items.ToList();
				if (newItems.Count == 0)
				{
						return;
				}

				suppressNotifications = true;
				try
				{
						for (int i = 0; i < newItems.Count; i++)
						{
								Items.Insert(index + i, newItems[i]);
						}
				}
				finally
				{
						suppressNotifications = false;
				}

				OnPropertyChanged(new(nameof(Count)));
				OnPropertyChanged(new("Item[]"));
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
				if (!suppressNotifications)
				{
						base.OnCollectionChanged(e);
				}
		}

		protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
		{
				if (!suppressNotifications)
				{
						base.OnPropertyChanged(e);
				}
		}
}
