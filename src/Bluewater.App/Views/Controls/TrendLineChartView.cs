using System;
using System.Collections.Generic;
using System.Linq;
using Bluewater.App.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Bluewater.App.Views.Controls;

public class TrendLineChartView : GraphicsView
{
  //public static readonly BindableProperty PointsProperty = BindableProperty.Create(
  //  nameof(Points),
  //  typeof(IReadOnlyList<DashboardTrendPoint>),
  //  typeof(TrendLineChartView),
  //  propertyChanged: OnPointsChanged);

  //public static readonly BindableProperty LineColorProperty = BindableProperty.Create(
  //  nameof(LineColor),
  //  typeof(Color),
  //  typeof(TrendLineChartView),
  //  Color.FromArgb("#2563EB"),
  //  propertyChanged: OnLineColorChanged);

  //public static readonly BindableProperty ShowYAxisLabelsProperty = BindableProperty.Create(
  //  nameof(ShowYAxisLabels),
  //  typeof(bool),
  //  typeof(TrendLineChartView),
  //  true,
  //  propertyChanged: OnShowYAxisLabelsChanged);

  //private readonly TrendLineChartDrawable drawable = new();

  //public TrendLineChartView()
  //{
  //  Drawable = drawable;
  //  HeightRequest = 220;
  //}

  //public IReadOnlyList<DashboardTrendPoint>? Points
  //{
  //  get => (IReadOnlyList<DashboardTrendPoint>?)GetValue(PointsProperty);
  //  set => SetValue(PointsProperty, value);
  //}

  //public Color LineColor
  //{
  //  get => (Color)GetValue(LineColorProperty);
  //  set => SetValue(LineColorProperty, value);
  //}

  //public bool ShowYAxisLabels
  //{
  //  get => (bool)GetValue(ShowYAxisLabelsProperty);
  //  set => SetValue(ShowYAxisLabelsProperty, value);
  //}

  //private static void OnPointsChanged(BindableObject bindable, object oldValue, object newValue)
  //{
  //  if (bindable is TrendLineChartView view)
  //  {
  //    view.drawable.UpdatePoints(newValue as IReadOnlyList<DashboardTrendPoint>);
  //    view.Invalidate();
  //  }
  //}

  //private static void OnLineColorChanged(BindableObject bindable, object oldValue, object newValue)
  //{
  //  if (bindable is TrendLineChartView view && newValue is Color color)
  //  {
  //    view.drawable.UpdateLineColor(color);
  //    view.Invalidate();
  //  }
  //}

  //private static void OnShowYAxisLabelsChanged(BindableObject bindable, object oldValue, object newValue)
  //{
  //  if (bindable is TrendLineChartView view && newValue is bool show)
  //  {
  //    view.drawable.UpdateShowYAxis(show);
  //    view.Invalidate();
  //  }
  //}

  //private sealed class TrendLineChartDrawable : IDrawable
  //{
  //  private IReadOnlyList<DashboardTrendPoint> points = Array.Empty<DashboardTrendPoint>();
  //  private Color lineColor = Color.FromArgb("#2563EB");
  //  private bool showYAxisLabels = true;

  //  public void UpdatePoints(IReadOnlyList<DashboardTrendPoint>? points)
  //  {
  //    this.points = points ?? Array.Empty<DashboardTrendPoint>();
  //  }

  //  public void UpdateLineColor(Color color)
  //  {
  //    lineColor = color;
  //  }

  //  public void UpdateShowYAxis(bool show)
  //  {
  //    showYAxisLabels = show;
  //  }

  //  public void Draw(ICanvas canvas, RectF dirtyRect)
  //  {
  //    canvas.SaveState();
  //    canvas.FillColor = Colors.Transparent;
  //    canvas.FillRectangle(dirtyRect);

  //    if (points.Count == 0)
  //    {
  //      DrawEmptyState(canvas, dirtyRect);
  //      canvas.RestoreState();
  //      return;
  //    }

  //    float leftPadding = 36f;
  //    float rightPadding = 16f;
  //    float bottomPadding = 40f;
  //    float topPadding = 24f;

  //    float left = dirtyRect.Left + leftPadding;
  //    float right = dirtyRect.Right - rightPadding;
  //    float bottom = dirtyRect.Bottom - bottomPadding;
  //    float top = dirtyRect.Top + topPadding;

  //    if (right <= left || bottom <= top)
  //    {
  //      canvas.RestoreState();
  //      return;
  //    }

  //    double maxValue = Math.Max(points.Max(point => point.Value), 1d);
  //    double range = Math.Max(maxValue, 1d);
  //    float width = right - left;
  //    float height = bottom - top;
  //    float step = points.Count <= 1 ? width : width / (points.Count - 1);

  //    canvas.StrokeSize = 1f;
  //    canvas.StrokeColor = Colors.Gray.WithAlpha(0.25f);
  //    canvas.DrawLine(left, bottom, right, bottom);
  //    canvas.DrawLine(left, top, left, bottom);

  //    canvas.FontColor = Colors.Gray;
  //    canvas.FontSize = 10f;

  //    if (showYAxisLabels)
  //    {
  //      string maxLabel = maxValue >= 100 ? ((int)Math.Round(maxValue)).ToString("N0") : maxValue.ToString("0.#");
  //      RectF maxLabelRect = new(left - 60f, top - 8f, 54f, 20f);
  //      RectF minLabelRect = new(left - 60f, bottom - 10f, 54f, 20f);
  //      canvas.DrawString(maxLabel, maxLabelRect, HorizontalAlignment.Right, VerticalAlignment.Center);
  //      canvas.DrawString("0", minLabelRect, HorizontalAlignment.Right, VerticalAlignment.Center);
  //    }

  //    PathF path = new();

  //    for (int i = 0; i < points.Count; i++)
  //    {
  //      float x = left + step * i;
  //      double clamped = Math.Clamp(points[i].Value, 0d, range);
  //      float y = bottom - (float)(clamped / range) * height;

  //      if (i == 0)
  //      {
  //        path.MoveTo(x, y);
  //      }
  //      else
  //      {
  //        path.LineTo(x, y);
  //      }

  //      canvas.FillColor = lineColor;
  //      canvas.FillCircle(x, y, 3.5f);

  //      RectF labelRect = new(x - 50f, bottom + 6f, 100f, 18f);
  //      canvas.FontColor = Colors.Gray;
  //      canvas.DrawString(points[i].Label, labelRect, HorizontalAlignment.Center, VerticalAlignment.Top);
  //    }

  //    canvas.StrokeColor = lineColor;
  //    canvas.StrokeSize = 2f;
  //    canvas.DrawPath(path);

  //    canvas.RestoreState();
  //  }

  //  private static void DrawEmptyState(ICanvas canvas, RectF bounds)
  //  {
  //    canvas.FontColor = Colors.Gray;
  //    canvas.FontSize = 14f;
  //    canvas.DrawString("No data", bounds, HorizontalAlignment.Center, VerticalAlignment.Center);
  //  }
  //}
}
