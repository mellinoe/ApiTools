using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if COREFX
using System.Composition;
#else
using System.ComponentModel.Composition;
#endif

namespace Microsoft.Cci.Differs
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportDifferenceRuleAttribute : ExportAttribute
    {
        public ExportDifferenceRuleAttribute()
            : base(typeof(IDifferenceRule))
        {
        }

        public bool NonAPIConformanceRule { get; set; }
        public bool MdilServicingRule { get; set; }
    }
}
