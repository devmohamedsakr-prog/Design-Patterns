using NUnit.Framework;
using Dashboard.After.Context;

namespace Dashboard.After.Tests
{
    [TestFixture]
    public class DashboardTests
    {
        private DataSource _source;
        private BarChart _chart;
        private LineGraph _graph;
        private StatisticsPanel _stats;

        [SetUp]
        public void Setup()
        {
            _source = new DataSource("Temperature", 20.0);
            _chart = new BarChart("TempChart");
            _graph = new LineGraph("TempGraph");
            _stats = new StatisticsPanel("TempStats");
        }

        [Test]
        public void Subscribe_Widget() { _source.Subscribe(_chart); Assert.That(_chart, Is.Not.Null); }

        [Test]
        public void Update_AllWidgets() { _source.Subscribe(_chart); _source.Subscribe(_graph); _source.Subscribe(_stats); _source.UpdateValue(25.0); Assert.That(_graph.DataPoints.Count, Is.EqualTo(2)); }

        [Test]
        public void BarChart_HighestValue() { _source.Subscribe(_chart); _source.UpdateValue(30.0); Assert.That(_chart.HighestValue, Is.EqualTo(30.0)); }

        [Test]
        public void Statistics_Average() { _source.Subscribe(_stats); _source.UpdateValue(25.0); _source.UpdateValue(35.0); Assert.That(_stats.AverageValue, Is.EqualTo(27.5)); }
    }
}
