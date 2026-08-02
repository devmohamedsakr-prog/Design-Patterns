using System;
using System.Collections.Generic;
using System.Linq;

namespace Dashboard.After.Context
{
    public interface IDashboardWidget
    {
        void Update(DataSource source);
        string GetWidgetName();
    }

    public class DataSource
    {
        private List<IDashboardWidget> _widgets = new();
        public string DataName { get; set; } = "";
        public double CurrentValue { get; set; }
        public DateTime LastUpdate { get; set; }

        public DataSource(string name, double initialValue)
        {
            DataName = name;
            CurrentValue = initialValue;
            LastUpdate = DateTime.Now;
        }

        public void Subscribe(IDashboardWidget widget)
        {
            if (!_widgets.Contains(widget))
            {
                _widgets.Add(widget);
                Console.WriteLine($"  ✓ {widget.GetWidgetName()} subscribed to {DataName}");
            }
        }

        public void UpdateValue(double newValue)
        {
            CurrentValue = newValue;
            LastUpdate = DateTime.Now;
            Console.WriteLine($"📈 {DataName} updated to {newValue}");
            foreach (var widget in _widgets.ToList())
                widget.Update(this);
        }
    }

    public class BarChart : IDashboardWidget
    {
        public string ChartName { get; set; }
        public double HighestValue { get; set; } = 0;

        public BarChart(string name)
        {
            ChartName = name;
        }

        public void Update(DataSource source)
        {
            if (source.CurrentValue > HighestValue)
                HighestValue = source.CurrentValue;
            Console.WriteLine($"    📊 {ChartName} updated: {source.DataName} = {source.CurrentValue}");
        }

        public string GetWidgetName() => ChartName;
    }

    public class LineGraph : IDashboardWidget
    {
        public string GraphName { get; set; }
        public List<double> DataPoints { get; set; } = new();

        public LineGraph(string name)
        {
            GraphName = name;
        }

        public void Update(DataSource source)
        {
            DataPoints.Add(source.CurrentValue);
            Console.WriteLine($"    📉 {GraphName} tracked: {source.CurrentValue}");
        }

        public string GetWidgetName() => GraphName;
    }

    public class StatisticsPanel : IDashboardWidget
    {
        public string PanelName { get; set; }
        public double AverageValue { get; set; }
        public List<double> Values { get; set; } = new();

        public StatisticsPanel(string name)
        {
            PanelName = name;
        }

        public void Update(DataSource source)
        {
            Values.Add(source.CurrentValue);
            AverageValue = Values.Average();
            Console.WriteLine($"    📊 {PanelName} stats: avg={AverageValue:F2}");
        }

        public string GetWidgetName() => PanelName;
    }
}
