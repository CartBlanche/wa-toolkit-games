namespace Microsoft.Samples.SocialGames.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class ExceptionAssert
    {
        public static void ShouldThrow<T>(Action action) where T : Exception
        {
            bool exceptionWasThrown = false;
            try
            {
                action();
            }
            catch (T)
            {
                exceptionWasThrown = true;
            }

            Assert.IsTrue(exceptionWasThrown);
        }

        public static void ShouldThrow<T1, T2>(Action action) where T1 : Exception where T2 : Exception
        {
            bool exceptionWasThrown = false;
            try
            {
                action();
            }
            catch (T1)
            {
                exceptionWasThrown = true;
            }
            catch (T2)
            {
                exceptionWasThrown = true;
            }

            Assert.IsTrue(exceptionWasThrown);
        }
    }
}
