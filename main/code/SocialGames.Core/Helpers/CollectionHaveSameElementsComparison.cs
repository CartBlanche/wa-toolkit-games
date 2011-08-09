namespace Microsoft.Samples.SocialGames.Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    public class CollectionHaveSameElementsComparison<T>
    {
        private IEnumerable<T> first;
        private IEnumerable<T> second;

        public CollectionHaveSameElementsComparison(IEnumerable<T> first, IEnumerable<T> second)
        {
            this.first = first;
            this.second = second;
        }

        public bool DoIt()
        {
            if (this.first.Count() == this.second.Count())
            {
                for (int i = 0; i < this.first.Count(); i++)
                {
                    if (!this.first.ElementAt(i).Equals(this.second.ElementAt(i)))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}