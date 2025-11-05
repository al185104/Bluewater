using System;
using System.Collections.Generic;
using System.Linq;
using Bluewater.App.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Bluewater.App.Views.Controls;

public class PieChartView : GraphicsView
{
  public static readonly BindableProperty SegmentsProperty = BindableProperty.Create(
    nameof(Segments),
    typeof(IReadOnlyList<DashboardChartSegment>),
    typeof(PieChartView),
    propertyChanged: OnSegmentsChanged);

  private readonly PieChartDrawable drawable = new();

  public PieChartView()
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
    if (bindable is PieChartView view)
    {
      view.drawable.UpdateSegments(newValue as IReadOnlyList<DashboardChartSegment>);
      view.Invalidate();
    }
  }

  private sealed class PieChartDrawable : IDrawable
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

      float diameter = Math.Min(dirtyRect.Width, dirtyRect.Height) * 0.9f;
      RectF pieRect = new(
        dirtyRect.Center.X - diameter / 2f,
        dirtyRect.Center.Y - diameter / 2f,
        diameter,
        diameter);

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
        canvas.FillColor = segment.Color.WithAlpha(0.85f);
        canvas.FillCircle(sweep, startAngle, startAngle);
        //canvas.FillArc(sweep, startAngle, startAngle, startAngle, startAngle);
        //canvas.FillPie(pieRect.X, pieRect.Y, pieRect.Width, pieRect.Height, startAngle, sweep);
        startAngle += sweep;
      }

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
