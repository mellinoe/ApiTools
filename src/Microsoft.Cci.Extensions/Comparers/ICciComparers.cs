using System.Collections.Generic;

namespace Microsoft.Cci.Comparers
{
    public interface ICciComparers
    {
        IEqualityComparer<T> GetEqualityComparer<T>();
        IComparer<T> GetComparer<T>();
    }
}
