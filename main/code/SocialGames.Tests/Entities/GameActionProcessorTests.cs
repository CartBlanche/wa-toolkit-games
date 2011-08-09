namespace Microsoft.Samples.SocialGames.Tests.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GameActionProcessorTests
    {
        [TestMethod]
        public void RegisterActionAndProcessCommand()
        {
            GameActionProcessor processor = new GameActionProcessor();

            int count = 0;

            processor.Register(gc => gc.Type == GameActionType.Shot, gc => { count++; });

            processor.Process(new GameAction() { Type = GameActionType.Shot });

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void RegisterTwoActionAndProcessCommand()
        {
            GameActionProcessor processor = new GameActionProcessor();

            int count = 0;

            processor.Register(gc => gc.Type == GameActionType.Shot, gc => { count++; });
            processor.Register(gc => gc.Type == GameActionType.Shot, gc => { count += 2; });

            processor.Process(new GameAction() { Type = GameActionType.Shot });

            Assert.AreEqual(3, count);
        }

        [TestMethod]
        public void RegisterActionWithTypeAndProcessCommand()
        {
            GameActionProcessor processor = new GameActionProcessor();

            int count = 0;

            processor.Register(GameActionType.Shot, gc => { count++; });

            processor.Process(new GameAction() { Type = GameActionType.Shot });

            Assert.AreEqual(1, count);
        }
    }
}
