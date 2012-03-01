using ENDA.PLCNetLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Security.Authentication;

namespace ENDA.PLCNetLibUnitTests
{
    
    
    /// <summary>
    ///This is a test class for PLCTest and is intended
    ///to contain all PLCTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PLCTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        static PLC target;
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            target = new PLC(ip, password);
        }
        
        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            target.Disconnect();
        }
        
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        //static IPEndPoint ip = new IPEndPoint(IPAddress.Parse("78.171.23.73"), 23);
        static IPEndPoint ip = new IPEndPoint(IPAddress.Parse("192.168.1.38"), 23);
        static string password = "4321";
        /// <summary>
        ///A test for Connect
        ///</summary>
        [TestMethod()]
        public void ConnectTest()
        {
            PLC target = new PLC(ip, password); // TODO: Initialize to an appropriate value
            target.Connect();
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidCredentialException),
        "Invalid password")]
        public void ConnectWrongPassTest()
        {
            PLC target = new PLC(ip, "3012893128"); // TODO: Initialize to an appropriate value
            target.Connect();
        }

        [TestMethod()]
        public void RunStopTest()
        {
            Assert.AreEqual(true, target.Run());
            Assert.AreEqual(true, target.Stop());
        }

        [TestMethod()]
        public void MITest()
        {
            target.MI[0] = Int32.MinValue;
            target.MI[1023] = Int32.MaxValue;
            Assert.AreEqual(Int32.MinValue, target.MI[0]);
            Assert.AreEqual(Int32.MaxValue, target.MI[1023]);
        }

        [TestMethod()]
        public void MFTest()
        {
            target.MF[0] = Single.MinValue;
            target.MF[1023] = Single.MaxValue;
            Assert.AreEqual(Single.MinValue, target.MF[0]);
            Assert.AreEqual(Single.MaxValue, target.MF[1023]);
        }

        [TestMethod()]
        public void TimeTest()
        {
            target.Time = DateTime.Now;
            TimeSpan ts = DateTime.Now - target.Time;
            Assert.AreEqual(0, ts.Seconds, 5);
        }

        [TestMethod()]
        public void MemTest()
        {
            for (int i = 0; i < 1024; i++)
                target.MI[i] = i * 10;
            for (int i = 0; i < 1024; i++)
                target.MF[i] = i * 11.0f;
            for (int i = 0; i < 512; i++)
                target.MW[i] = (ushort)(i * 12);
            for (int i = 0; i < 1024; i++)
                target.MB[i] = (i % 2) == 1 ? true : false;
            for (int i = 0; i < 1024; i++)
                target.QP[i] = (i % 2) == 1 ? true : false;
            for (int i = 0; i < 1024; i++)
                Assert.AreEqual(i * 10, target.MI[i]);
            for (int i = 0; i < 1024; i++)
                Assert.AreEqual(i * 11.0f, target.MF[i]);
            for (int i = 0; i < 512; i++)
                Assert.AreEqual((ushort)(i * 12), target.MW[i]);
            for (int i = 0; i < 1024; i++)
                Assert.AreEqual((i % 2) == 1 ? true : false, target.MB[i]);
            for (int i = 0; i < 1024; i++)
                Assert.AreEqual((i % 2) == 1 ? true : false, target.QP[i]);
        }
    }
}
