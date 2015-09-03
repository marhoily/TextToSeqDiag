using System.Windows.Media;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TextToSeqDiag
{
    [TestClass]
    [UseReporter(typeof(AraxisMergeReporter))]
    public class SequenceDiagramTests
    {
        private SequenceDiagram _view;

        [TestInitialize]
        public void TestInitialize()
        {
            _view = new SequenceDiagram();
        }

        [TestMethod]
        public void Verify_Square_Looks()
        {
            _view.AddActor("Server");
            _view.VerifySnapshot();
        }
    }
}
