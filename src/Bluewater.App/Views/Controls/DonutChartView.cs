using System;
using System.Collections.Generic;
using System.Linq;
using Bluewater.App.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Bluewater.App.Views.Controls;

public class DonutChartView : GraphicsView
{
  public static readonly BindableProperty SegmentsProperty = BindableProperty.Create(
    nameof(Segments),
    typeof(IReadOnlyList<DashboardChartSegment>),
    typeof(DonutChartView),
    propertyChanged: OnSegmentsChanged);

  private readonly DonutChartDrawable drawable = new();

  public DonutChartView()
  {
    Drawable = drawable;
    HeightRequest = 220;
  }

  public IReadOnlyList<DashboardChartSegment>? Segments
  {
    get => (IReadOnlyList<DashboardChartSegment>?)GetValue(SegmentsProperty);
    set => SetValue(SegmentsProperty, value);
  }

  private static void OnSegmentsChanged(BindableObject bindable, object oldValue, object newValue)
  {
    if (bindable is DonutChartView view)
    {
      view.drawable.UpdateSegments(newValue as IReadOnlyList<DashboardChartSegment>);
      view.Invalidate();
    }
  }

  private sealed class DonutChartDrawable : IDrawable
  {
    private IReadOnlyList<DashboardChartSegment> segments = Array.Empty<DashboardChartSegment>();

    public void UpdateSegments(IReadOnlyList<DashboardChartSegment>? segments)
    {
      this.segments = segments ?? Array.Empty<DashboardChartSegment>();
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
      canvas.SaveState();
      canvas.FillColor = Colors.Transparent;
      canvas.FillRectangle(dirtyRect);

      if (segments.Count == 0 || segments.All(segment => segment.Value <= 0))
      {
        DrawEmptyState(canvas, dirtyRect);
        canvas.RestoreState();
        return;
      }

      float strokeWidth = Math.Min(dirtyRect.Width, dirtyRect.Height) * 0.22f;
      float radius = (Math.Min(dirtyRect.Width, dirtyRect.Height) - strokeWidth) / 2f;
      PointF center = new(dirtyRect.Center.X, dirtyRect.Center.Y);

      double total = segments.Sum(segment => Math.Max(segment.Value, 0d));
      float startAngle = -90f;

      foreach (DashboardChartSegment segment in segments)
      {
        double value = Math.Max(segment.Value, 0d);
        if (total <= 0 || value <= 0)
        {
          continue;
        }

        float sweep = (float)(value / total * 360d);
        canvas.StrokeColor = segment.Color;
        canvas.StrokeSize = strokeWidth;
        canvas.DrawArc(
          center.X - radius,
          center.Y - radius,
          radius * 2f,
          radius * 2f,
          startAngle,
          startAngle + sweep,
          false,
          false);

        startAngle += sweep;
      }

      string totalText = total >= 100 ? ((int)Math.Round(total)).ToString("N0") : total.ToString("0.##");
      canvas.FontColor = Colors.Gray;
      canvas.FontSize = 18;
      canvas.DrawString(totalText, new RectF(center.X - 80, center.Y - 24, 160, 48), HorizontalAlignment.Center, VerticalAlignment.Center);

      canvas.RestoreState();
    }

    private static void DrawEmptyState(ICanvas canvas, RectF bounds)
    {
      canvas.FontColor = Colors.Gray;
      canvas.FontSize = 14;
      canvas.DrawString("No data", bounds, HorizontalAlignment.Center, VerticalAlignment.Center);
    }
  }
}
