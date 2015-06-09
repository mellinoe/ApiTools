using Microsoft.Cci.Differs;

namespace Microsoft.Cci.Filters
{
    public interface IDifferenceFilter
    {
        bool Include(Difference difference);
    }
}
