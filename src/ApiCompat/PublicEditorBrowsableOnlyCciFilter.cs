using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Cci;
using Microsoft.Cci.Extensions;
using Microsoft.Cci.Filters;

namespace ApiCompat
{
    // This filter is activated when ApiCompat is used to detect changes that validate public types except those that are not EditorBrowsable.
    //
    internal class PublicEditorBrowsableOnlyCciFilter : PublicOnlyCciFilter
    {
        private HashSet<string> typeExclusions = new HashSet<string>();

        public PublicEditorBrowsableOnlyCciFilter(bool excludeAttributes = true)
            : base(excludeAttributes)
        {
        }

        public override bool Include(ITypeDefinition type)
        {
            if (!base.Include(type))
            {
                return false;
            }

            return !Exclude(type, this.typeExclusions);
        }

        private static bool Exclude(IReference reference, HashSet<string> exclusions, string alternateName = null)
        {
            string name = reference.FullName();
            bool excluded = exclusions.Contains(name);

            if (!excluded && alternateName != null)
            {
                excluded = exclusions.Contains(alternateName);
            }

            bool exclude = excluded || (reference.Attributes != null
                && reference.Attributes.Any(attribute => attribute.IsEditorBrowseableStateNever()));

            if (exclude && !excluded)
            {
                exclusions.Add(name);
            }

            return exclude;
        }
    }
}