using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Cci.Extensions;
using Microsoft.Cci.Extensions.CSharp;

namespace Microsoft.Cci.Differs.Rules
{
    // Removed as MembersMustExist supercededs this.
    //[ExportDifferenceRule]
    internal class MemberReturnTypesMustMatch : DifferenceRule
    {
        [Import]
        private IEqualityComparer<ITypeReference> _typeComparer = null;

        public override DifferenceType Diff(IDifferences differences, ITypeDefinitionMember impl, ITypeDefinitionMember contract)
        {
            if (impl == null || contract == null)
                return DifferenceType.Unknown;

            if (!ReturnTypesMatch(differences, impl, contract))
                return DifferenceType.Changed;

            return DifferenceType.Unknown;
        }

        public bool ReturnTypesMatch(IDifferences differences, ITypeDefinitionMember impl, ITypeDefinitionMember contract)
        {
            ITypeReference implType = impl.GetReturnType();
            ITypeReference contractType = contract.GetReturnType();

            if (implType == null || contractType == null)
                return true;

            if(!_typeComparer.Equals(implType, contractType))
            {
                differences.AddTypeMismatchDifference(this, implType, contractType,
                    "Return type on member '{0}' is '{1}' in the implementation but '{2}' in the contract.", 
                    impl.FullName(), implType.FullName(), contractType.FullName());
                return false;
            }

            return true;
        }
    }
}
