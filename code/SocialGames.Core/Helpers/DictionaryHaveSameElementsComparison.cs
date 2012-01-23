namespace Microsoft.Samples.SocialGames.Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    public class DictionaryHaveSameElementsComparison<T, K>
    {
        private IDictionary<T, K> first;

        private IDictionary<T, K> second;

        public DictionaryHaveSameElementsComparison(IDictionary<T, K> first, IDictionary<T, K> second)
        {
            this.first = first;
            this.second = second;
        }

        public bool DoIt()
        {
            if (this.first.Count == this.second.Count)
            {
                for (int i = 0; i < this.first.Count; i++)
                {
                    if (!this.first.ElementAt(i).Key.Equals(this.second.ElementAt(i).Key) || 
                        !this.first.ElementAt(i).Value.Equals(this.second.ElementAt(i).Value))
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