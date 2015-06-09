using System.Collections.Generic;
using Microsoft.Cci.Extensions;

namespace Microsoft.Cci.Filters
{
    public class ExcludeAttributesFilter : PublicOnlyCciFilter
    {
        private readonly HashSet<string> _attributeDocIds;

        public ExcludeAttributesFilter(IEnumerable<string> attributeDocIds)
            : base(false)
        {
            _attributeDocIds = new HashSet<string>(attributeDocIds);
        }

        public ExcludeAttributesFilter(string attributeDocIdFile)
            : base(false)
        {
            _attributeDocIds = DocIdExtensions.ReadDocIds(attributeDocIdFile);
        }

        public override bool Include(ICustomAttribute attribute)
        {
            if (_attributeDocIds.Contains(attribute.DocId()))
                return false;

            return base.Include(attribute);
        }
    }
}
