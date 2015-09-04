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
        public void Verify_One_Actor()
        {
            _view.AddActor("Server");
            _view.VerifySnapshot();
        }

        [TestMethod]
        public void Verify_Two_Actors()
        {
            _view.AddActor("Server");
            _view.AddActor("Client");
            _view.VerifySnapshot();
        }
    }
}
