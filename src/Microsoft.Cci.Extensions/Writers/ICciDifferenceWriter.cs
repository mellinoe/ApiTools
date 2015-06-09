using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Cci.Mappings;

namespace Microsoft.Cci.Writers
{
    public interface ICciDifferenceWriter
    {
        void Write(string oldAssembliesName, IEnumerable<IAssembly> oldAssemblies, string newAssembliesName, IEnumerable<IAssembly> newAssemblies);
    }
}
