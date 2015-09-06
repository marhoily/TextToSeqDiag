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
            _view.AddActor("C");
            _view.VerifySnapshot();
        }

        [TestMethod]
        public void Verify_Message()
        {
            _view.AddActor("Server");
            _view.AddActor("Client");
            _view.AddMessage(0, 1, "Hello!");
            _view.VerifySnapshot();
        }

        [TestMethod]
        public void Verify_Message_That_Goes_Through_Column()
        {
            _view.AddActor("Server");
            _view.AddActor("X");
            _view.AddActor("Client");
            _view.AddMessage(0, 2, "Hello!");
            _view.VerifySnapshot();
        }

        [TestMethod]
        public void Verify_Message_To_The_Left()
        {
            _view.AddActor("Server");
            _view.AddActor("Client");
            _view.AddMessage(1, 0, "Hello!");
            _view.VerifySnapshot();
        }

        [TestMethod]
        public void Verify_Messages_Vertical_Compactification()
        {
            _view.AddActor("Curstomer");
            _view.AddActor("Server");
            _view.AddActor("Employee");
            _view.AddMessage(0, 1, "Hello!");
            _view.AddMessage(1, 2, "Hello!");
            _view.AddMessage(2, 0, "Bye!");
            _view.VerifySnapshot();
        }
    }
}
