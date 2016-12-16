using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using update.net;
using System.Threading;

namespace update.net_test
{
    [TestClass]
    public class HTTPPlaintextUpdaterTest
    {
        static string BIN_DIR = Directory.GetCurrentDirectory();
        static string PROJ_DIR = Directory.GetParent(BIN_DIR).Parent.FullName;
        static string SERVER_DIR = Path.Combine(PROJ_DIR, "TestServer");
        static string LOCAL_DIR = Path.Combine(PROJ_DIR, "TestLocal");
        static string UPDATER_URL = Path.Combine(SERVER_DIR, "updater.txt");
        static string VERSION_URL = Path.Combine(SERVER_DIR, "version.txt");
        static string CHANGELOG_URL = Path.Combine(SERVER_DIR, "changelog.txt");

        private HTTPPlaintextUpdater updater;

        [TestInitialize]
        public void Initialize()
        {
            this.updater = new HTTPPlaintextUpdater(
                VERSION_URL,
                UPDATER_URL,
                LOCAL_DIR,
                CHANGELOG_URL);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException),
            "Providing a null version url did not throw")]
        public void TestConstructorThrowsWhenNoVersionURL()
        {
            HTTPPlaintextUpdater updater = new HTTPPlaintextUpdater(
                null,
                "updater_url",
                "directory");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException),
            "Providing a null updater url did not throw")]
        public void TestConstructorThrowsWhenNoUpdaterURL()
        {
            HTTPPlaintextUpdater updater = new HTTPPlaintextUpdater(
                "version_url",
                null,
                "directory");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException),
            "Providing a null directory did not throw")]
        public void TestConstructorThrowsWhenNoDirectory()
        {
            HTTPPlaintextUpdater updater = new HTTPPlaintextUpdater(
                "version_url",
                "updater_url",
                null);
        }

        [TestMethod]
        public void TestGetLatestVersion()
        {
            int latestVersion = this.updater.GetLatestVersion();
            Assert.AreEqual(50, latestVersion);
        }

        [TestMethod]
        public void TestIsUpdateAvailable()
        {
            Assert.IsTrue(this.updater.IsUpdateAvailable(49));
            Assert.IsFalse(this.updater.IsUpdateAvailable(51));
        }

        [TestMethod]
        public void TestDownloadUpdate()
        {
            Semaphore updateDownloaded = new Semaphore(0, 1);
            this.updater.UpdateDownloaded += (sender, e) =>
            {
                updateDownloaded.Release();
            };

            this.updater.DownloadUpdate();

            Assert.IsTrue(updateDownloaded.WaitOne(TimeSpan.FromMilliseconds(500)),
                "Update not downloaded succesfully");

            string updatePath = Path.Combine(LOCAL_DIR, "updater.txt");
            var updateReader = new StreamReader(updatePath);
            Assert.AreEqual("update", updateReader.ReadToEnd());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetChangelogNoURL()
        {
            var updater = new HTTPPlaintextUpdater(
                VERSION_URL,
                UPDATER_URL,
                SERVER_DIR);
            updater.GetChangelog();
        }

        [TestMethod]
        public void TestGetChangelog()
        {
            Assert.AreEqual("changelog", this.updater.GetChangelog());
        }
    }
}
