using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using NLog;
using NLog.Targets;
using NLogFactoryForUnity;


// The purpose of these test cases is making sure that the logger is correctly configured, not output correctly.

namespace NLogFactoryForUnityTests
{
    [TestClass]
    public class TestNLogFactory
    {
        [TestMethod]
        public void TestCreateUnityConsoleTarget()
        {
            // CreateUnityConsoleTarget
            var ucTargetWithoutArgs = NLogFactory.CreateUnityConsoleTarget();
            Assert.AreEqual(null, ucTargetWithoutArgs.Name);
            Assert.AreEqual("'[${level:uppercase=true}] [${callsite}] ${message}${newline}${exception:format=Message, ToString:separator=\n}'", ucTargetWithoutArgs.Layout.ToString());

            var ucTargetWithName = NLogFactory.CreateUnityConsoleTarget("UCTarget");
            Assert.AreEqual("UCTarget", ucTargetWithName.Name);
            Assert.AreEqual("'[${level:uppercase=true}] [${callsite}] ${message}${newline}${exception:format=Message, ToString:separator=\n}'", ucTargetWithName.Layout.ToString());

            var ucTargetWithLayout = NLogFactory.CreateUnityConsoleTarget("UCTarget", "${message}");
            Assert.AreEqual("UCTarget", ucTargetWithLayout.Name);
            Assert.AreEqual("'${message}'", ucTargetWithLayout.Layout.ToString());
        }

        [TestMethod]
        public void TestCreateJsonWebServiceTarget()
        {
            var wsTargetWithUrl = NLogFactory.CreateJsonWebServiceTarget("http://localhost:5001/nlog");
            Assert.AreEqual(null, wsTargetWithUrl.Name);
            Assert.AreEqual("http://localhost:5001/nlog", wsTargetWithUrl.Url.OriginalString);
            Assert.AreEqual(6, wsTargetWithUrl.Parameters.Count);
            Assert.AreEqual(WebServiceProtocol.JsonPost, wsTargetWithUrl.Protocol);

            var wsTargetWithName = NLogFactory.CreateJsonWebServiceTarget("WSTarget", "http://localhost:5001/nlog");
            Assert.AreEqual("WSTarget", wsTargetWithName.Name);
            Assert.AreEqual("http://localhost:5001/nlog", wsTargetWithName.Url.OriginalString);
            Assert.AreEqual(6, wsTargetWithName.Parameters.Count);
            Assert.AreEqual(WebServiceProtocol.JsonPost, wsTargetWithName.Protocol);

            var jsonParams = new Dictionary<string, string>()
            {
                {"param1", "value1"},
                {"param2", "value2"}
            };

            var wsTargetWithParams = NLogFactory.CreateJsonWebServiceTarget("http://localhost:5001/nlog", jsonParams);
            Assert.AreEqual(null, wsTargetWithParams.Name);
            Assert.AreEqual("http://localhost:5001/nlog", wsTargetWithParams.Url.OriginalString);
            Assert.AreEqual(2, wsTargetWithParams.Parameters.Count);
            Assert.AreEqual(WebServiceProtocol.JsonPost, wsTargetWithParams.Protocol);

            var wsTargetWithNameAndParams = NLogFactory.CreateJsonWebServiceTarget("WSTarget", "http://localhost:5001/nlog", jsonParams);
            Assert.AreEqual("WSTarget", wsTargetWithNameAndParams.Name);
            Assert.AreEqual("http://localhost:5001/nlog", wsTargetWithNameAndParams.Url.OriginalString);
            Assert.AreEqual(2, wsTargetWithNameAndParams.Parameters.Count);
            Assert.AreEqual(WebServiceProtocol.JsonPost, wsTargetWithNameAndParams.Protocol);
        }

        [TestMethod]
        public void TestGetLoggerWithConfigFile()
        {
            var configPath = @"NLog.config";
            var logger = NLogFactory.GetLogger(configPath);
            var config = LogManager.Configuration;

            // UnityConsoleTarget
            var ucTarget = config.FindTargetByName<UnityConsoleTarget>("UCTarget");
            Assert.AreEqual("'[${level:uppercase=true}] [${callsite}] ${message}${newline}${exception:format=Message, ToString:separator=\n}'", ucTarget.Layout.ToString());

            // WebServiceTarget
            var wsTarget = config.FindTargetByName<WebServiceTarget>("JWSTarget");
            Assert.AreEqual("http://localhost:5001/nlog", wsTarget.Url.OriginalString);
            Assert.AreEqual(6, wsTarget.Parameters.Count);
            Assert.AreEqual(WebServiceProtocol.JsonPost, wsTarget.Protocol);
        }

        [TestMethod]
        public void TestGetLoggerWithConfigFileAndName()
        {
            var configPath = @"NLog.config";
            var logger = NLogFactory.GetLogger("TestLogger", configPath);
            var config = LogManager.Configuration;

            // Logger name
            Assert.AreEqual("TestLogger", logger.Name);

            // UnityConsoleTarget
            var ucTarget = config.FindTargetByName<UnityConsoleTarget>("UCTarget");
            Assert.AreEqual("'[${level:uppercase=true}] [${callsite}] ${message}${newline}${exception:format=Message, ToString:separator=\n}'", ucTarget.Layout.ToString());

            // WebServiceTarget
            var wsTarget = config.FindTargetByName<WebServiceTarget>("JWSTarget");
            Assert.AreEqual("http://localhost:5001/nlog", wsTarget.Url.OriginalString);
            Assert.AreEqual(6, wsTarget.Parameters.Count);
            Assert.AreEqual(WebServiceProtocol.JsonPost, wsTarget.Protocol);
        }

        [TestMethod]
        public void TestGetLoggerWithConfigFileAndEventProperties()
        {
            var configPath = @"NLog.config";
            var eventProperties = new Dictionary<string, object>(){
                { "prop1", "value1"},
                { "prop2", 123}
            };

            var logger = NLogFactory.GetLogger(configPath, eventProperties);
            var config = LogManager.Configuration;

            // UnityConsoleTarget
            var ucTarget = config.FindTargetByName<UnityConsoleTarget>("UCTarget");
            Assert.AreEqual("'[${level:uppercase=true}] [${callsite}] ${message}${newline}${exception:format=Message, ToString:separator=\n}'", ucTarget.Layout.ToString());

            // WebServiceTarget
            var wsTarget = config.FindTargetByName<WebServiceTarget>("JWSTarget");
            Assert.AreEqual("http://localhost:5001/nlog", wsTarget.Url.OriginalString);
            Assert.AreEqual(6, wsTarget.Parameters.Count);
            Assert.AreEqual(WebServiceProtocol.JsonPost, wsTarget.Protocol);

            // EventParams
            Assert.AreEqual("value1", logger.Properties["prop1"]);
            Assert.AreEqual(123, logger.Properties["prop2"]);
        }

        [TestMethod]
        public void TestGetLoggerWithConfigFileEventPropertiesAndName()
        {
            var configPath = @"NLog.config";
            var eventProperties = new Dictionary<string, object>(){
                { "prop1", "value1"},
                { "prop2", 123}
            };

            var logger = NLogFactory.GetLogger("TestLogger", configPath, eventProperties);
            var config = LogManager.Configuration;

            // Logger name
            Assert.AreEqual("TestLogger", logger.Name);

            // UnityConsoleTarget
            var ucTarget = config.FindTargetByName<UnityConsoleTarget>("UCTarget");
            Assert.AreEqual("'[${level:uppercase=true}] [${callsite}] ${message}${newline}${exception:format=Message, ToString:separator=\n}'", ucTarget.Layout.ToString());

            // WebServiceTarget
            var wsTarget = config.FindTargetByName<WebServiceTarget>("JWSTarget");
            Assert.AreEqual("http://localhost:5001/nlog", wsTarget.Url.OriginalString);
            Assert.AreEqual(6, wsTarget.Parameters.Count);
            Assert.AreEqual(WebServiceProtocol.JsonPost, wsTarget.Protocol);

            // EventParams
            Assert.AreEqual("value1", logger.Properties["prop1"]);
            Assert.AreEqual(123, logger.Properties["prop2"]);
        }


        [TestMethod]
        public void TestWrite()
        {
            var configPath = @"NLog.config";
            var logger = NLogFactory.GetLogger(configPath);
            var config = NLog.LogManager.Configuration;

            var mTarget = config.FindTargetByName<MemoryTarget>("MTarget");

            logger.Trace("test");
            logger.Debug("test");
            logger.Info("test");
            logger.Warn("test");
            logger.Error("test");
            logger.Fatal("test");
            Assert.AreEqual("[TRACE] test", mTarget.Logs[0]);
            Assert.AreEqual("[DEBUG] test", mTarget.Logs[1]);
            Assert.AreEqual("[INFO] test", mTarget.Logs[2]);
            Assert.AreEqual("[WARN] test", mTarget.Logs[3]);
            Assert.AreEqual("[ERROR] test", mTarget.Logs[4]);
            Assert.AreEqual("[FATAL] test", mTarget.Logs[5]);
        }


        //[TestMethod]
        public void TestGetLoggerConfiguredByCode()
        {
            var logger = NLogFactory.GetLogger(ConfigureLogger);
            var config = LogManager.Configuration;

            // UnityConsoleTarget
            var ucTarget = config.FindTargetByName<UnityConsoleTarget>("UCTargetByCode");
            Assert.AreEqual("'[${level:uppercase=true}] [${callsite}] ${message}${newline}${exception:format=Message, ToString:separator=\n}'", ucTarget.Layout.ToString());

            // WebServiceTarget
            var wsTarget = config.FindTargetByName<WebServiceTarget>("JWSTargetByCode");
            Assert.AreEqual("http://localhost:1234/nlog", wsTarget.Url.OriginalString);
            Assert.AreEqual(2, wsTarget.Parameters.Count);
            Assert.AreEqual(WebServiceProtocol.JsonPost, wsTarget.Protocol);

            // MemoryTarget
            var mTarget = config.FindTargetByName<MemoryTarget>("MTargetByCode");

            // Test writing
            logger.Trace("test");
            logger.Debug("test");
            logger.Info("test");
            logger.Warn("test");
            logger.Error("test");
            logger.Fatal("test");
            Assert.AreEqual("[TRACE] test", mTarget.Logs[0]);
            Assert.AreEqual("[DEBUG] test", mTarget.Logs[1]);
            Assert.AreEqual("[INFO] test", mTarget.Logs[2]);
            Assert.AreEqual("[WARN] test", mTarget.Logs[3]);
            Assert.AreEqual("[ERROR] test", mTarget.Logs[4]);
            Assert.AreEqual("[FATAL] test", mTarget.Logs[5]);

            Assert.AreEqual(3, config.LoggingRules.Count);
        }

        void ConfigureLogger(NLog.Config.LoggingConfiguration config)
        {
            var jsonParams = new Dictionary<string, string>()
            {
                {"hoge", "fuga"},
                {"foo", "bar"}
            };

            // initialize targets
            var ucTarget = NLogFactory.CreateUnityConsoleTarget("UCTargetByCode");
            var wsTarget = NLogFactory.CreateJsonWebServiceTarget("JWSTargetByCode", "http://localhost:1234/nlog", jsonParams);
            var mTarget = new MemoryTarget("MTargetByCode");
            mTarget.Layout = "[${level:uppercase=true}] ${message}";

            // add rules
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, ucTarget);
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, wsTarget);
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, mTarget);
        }
    }
}
