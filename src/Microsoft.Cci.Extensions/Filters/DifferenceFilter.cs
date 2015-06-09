using Microsoft.Cci.Differs;

namespace Microsoft.Cci.Filters
{
    public class DifferenceFilter<T> : IDifferenceFilter where T : Difference
    {
        public virtual bool Include(Difference difference)
        {
            return difference is T;
        }
    }
}
