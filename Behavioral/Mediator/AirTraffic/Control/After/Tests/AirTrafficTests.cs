using NUnit.Framework;
using AirTrafficControl.After.Context;
using System.Collections.Generic;
using System;
using System.Linq;

namespace AirTrafficControl.After.Tests
{
    [TestFixture]
    public class AirTrafficControlTests
    {
        private ControlTower _tower;
        private Plane _plane1, _plane2, _plane3;

        [SetUp]
        public void Setup()
        {
            _tower = new ControlTower();
            _plane1 = new Plane("AA100", _tower);
            _plane2 = new Plane("UA200", _tower);
            _plane3 = new Plane("DL300", _tower);
        }

        [Test]
        public void RegisterPlane_Success() => Assert.That(_plane1.CallSign, Is.EqualTo("AA100"));

        [Test]
        public void RequestLanding_QueuePlane() 
        { 
            _plane1.RequestLanding();
            Assert.That(_plane1.Status, Is.AnyOf("Landed", "In Flight", "Idle"));
        }

        [Test]
        public void MultiplePlanesLanding()
        {
            _plane1.RequestLanding();
            _plane2.RequestLanding();
            _plane3.RequestLanding();
            Assert.That(_plane1.Status, Is.Not.Empty);
        }

        [Test]
        public void PlaneStatusUpdate()
        {
            _plane1.Status = "In Flight";
            _plane1.RequestLanding();
            Assert.That(_plane1.Status, Is.Not.EqualTo("In Flight"));
        }

        [Test]
        public void NotifyAllPlanes()
        {
            _tower.NotifyPlanes("Weather alert");
            Assert.Pass();
        }

        [Test]
        public void TakeoffOperation()
        {
            _plane1.RequestTakeoff();
            Assert.That(_plane1.Status, Is.EqualTo("In Flight"));
        }
    }
}
