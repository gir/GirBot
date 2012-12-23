//------------------------------------------------------------------------------
// <copyright file="UrlTitleTests.cs" company="non.io">
// © non.io
// </copyright>
//------------------------------------------------------------------------------

namespace UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using UrlTitle;

    /// <summary>
    /// UrlTitleTests class.
    /// </summary>
    [TestClass]
    public class UrlTitleTests
    {
        /// <summary>
        /// Tests GetPageTitle.
        /// </summary>
        [TestMethod]
        public void TestGetPageTitle()
        {
            Assert.IsTrue(Plugin.GetPageTitle("http://google.com").Equals("Google"));
            Assert.IsTrue(Plugin.GetPageTitle("https://soundcloud.com/ryan-nellis/haben-sie-ein-telefon").Equals("Haben Sie Ein Telefon(Rammstein vs. Lady Gaga) by Ryan Nellis Mashups on SoundCloud - Hear the world’s sounds"));
        }

        /// <summary>
        /// Tests GetPageTitle when the input is not a url.
        /// </summary>
        [TestMethod]
        public void TestGetPageTitleNotUrl()
        {
            Assert.IsNull(Plugin.GetPageTitle("test"));
        }

        /// <summary>
        /// Tests GetPageTitle when the input is a PDF.
        /// </summary>
        [TestMethod]
        public void TestGetPageTitlePdf()
        {
            Assert.IsNull(Plugin.GetPageTitle("http://d284f45nftegze.cloudfront.net/kayasushi/KAYA%202012.pdf"));
        }
    }
}