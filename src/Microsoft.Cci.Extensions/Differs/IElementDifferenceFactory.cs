using Microsoft.Cci.Mappings;

namespace Microsoft.Cci.Differs
{
    public interface IElementDifferenceFactory
    {
        IDifferences GetDiffer<T>(ElementMapping<T> element) where T : class;
    }
}
