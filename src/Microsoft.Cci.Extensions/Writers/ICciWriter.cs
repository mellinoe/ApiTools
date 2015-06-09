using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Cci.Writers
{
    public interface ICciWriter
    {
        void WriteAssemblies(IEnumerable<IAssembly> assemblies);
    }
}
