using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TextToSeqDiag
{
    [TestClass]
    [UseReporter(typeof(AraxisMergeReporter))]
    public class GroupSymbolControlTests
    {
        private UserControl1 _view;

        [TestInitialize]
        public void TestInitialize()
        {
            _view = new UserControl1
            {
                Width = 80,
                Height = 80,
                BorderBrush = Brushes.Black,
            };
        }

        [TestMethod]
        public void Verify_Square_Looks()
        {
            _view.VerifySnapshot();
        }
    }
}
