using ENDA.PLCNetLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.IO;

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
        static PLC plc;
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            plc = new PLC(ip, password);
        }
        
        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            plc.Disconnect();
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
        static IPEndPoint ip = new IPEndPoint(IPAddress.Parse("192.168.1.96"), 23);
        static string password = "1234";
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
            Assert.AreEqual(true, plc.Run());
            Assert.AreEqual(true, plc.Stop());
        }

        [TestMethod()]
        public void MITest()
        {
            plc.MI[0] = Int32.MinValue;
            plc.MI[1023] = Int32.MaxValue;
            Assert.AreEqual(Int32.MinValue, plc.MI[0]);
            Assert.AreEqual(Int32.MaxValue, plc.MI[1023]);
        }

        [TestMethod()]
        public void MFTest()
        {
            plc.MF[0] = Single.MinValue;
            plc.MF[1023] = Single.MaxValue;
            Assert.AreEqual(Single.MinValue, plc.MF[0]);
            Assert.AreEqual(Single.MaxValue, plc.MF[1023]);
        }

        [TestMethod()]
        public void TimeTest()
        {
            plc.Time = DateTime.Now;
            TimeSpan ts = DateTime.Now - plc.Time;
            Assert.AreEqual(0, ts.Seconds, 5);
        }

        [TestMethod()]
        public void CmdAsyncTest()
        {
            IAsyncResult ar = plc.BeginCmd(ASCIIEncoding.ASCII.GetBytes("help"), null, null);
            ar.AsyncWaitHandle.WaitOne();
            Response resp = plc.EndCmd(ar);
            string str = resp.String;
            Assert.AreEqual(true, resp.String.StartsWith("Available commands:"));
        }

        [TestMethod()]
        public void ReadWriteRawAsyncTest()
        {
            const int INT_COUNT = 200;
            MemoryStream ms = new MemoryStream(INT_COUNT * 4);
            BinaryWriter bw = new BinaryWriter(ms);
            for (int i = 0; i < INT_COUNT; i++)
                bw.Write(i * 2);
            IAsyncResult ar = plc.BeginWriteRaw(0, ms.GetBuffer(), null, null);
            ar.AsyncWaitHandle.WaitOne();
            plc.EndWrite(ar);

            ar = plc.BeginRead(0, INT_COUNT * 4, null, null);
            ar.AsyncWaitHandle.WaitOne();
            BinaryReader br = plc.EndRead(ar);
            for (int i = 0; i < INT_COUNT; i++)
                Assert.AreEqual(i * 2, br.ReadInt32());
        }

        [TestMethod()]
        public void MemTest()
        {
            for (int i = 0; i < 1024; i++)
                plc.MI[i] = i * 10;
            for (int i = 0; i < 1024; i++)
                plc.MF[i] = i * 11.0f;
            for (int i = 0; i < 512; i++)
                plc.MW[i] = (ushort)(i * 12);
            for (int i = 0; i < 1024; i++)
                plc.MB[i] = (i % 2) == 1 ? true : false;
            for (int i = 0; i < 1024; i++)
                plc.QP[i] = (i % 2) == 1 ? true : false;
            for (int i = 0; i < 1024; i++)
                Assert.AreEqual(i * 10, plc.MI[i]);
            for (int i = 0; i < 1024; i++)
                Assert.AreEqual(i * 11.0f, plc.MF[i]);
            for (int i = 0; i < 512; i++)
                Assert.AreEqual((ushort)(i * 12), plc.MW[i]);
            for (int i = 0; i < 1024; i++)
                Assert.AreEqual((i % 2) == 1 ? true : false, plc.MB[i]);
            for (int i = 0; i < 1024; i++)
                Assert.AreEqual((i % 2) == 1 ? true : false, plc.QP[i]);
        }
    }
}
