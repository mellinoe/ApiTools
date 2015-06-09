using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Cci.Filters
{
    public interface ICciFilter
    {
        bool Include(INamespaceDefinition ns);
        bool Include(ITypeDefinition type);
        bool Include(ITypeDefinitionMember member);
        bool Include(ICustomAttribute attribute); // Used to filter the application of attributes as opposed to attribute types
    }
}
