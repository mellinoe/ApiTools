using System;
using Microsoft.Cci.Differs;
using Microsoft.Cci.Mappings;

namespace Microsoft.Cci.Filters
{
    public class TypesOnlyMappingDifferenceFilter : MappingDifferenceFilter
    {
        public TypesOnlyMappingDifferenceFilter(Func<DifferenceType, bool> include, ICciFilter filter)
            : base(include, filter)
        {
        }

        public override bool Include(MemberMapping member)
        {
            return false;
        }
    }
}
