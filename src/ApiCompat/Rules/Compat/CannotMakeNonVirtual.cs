using Microsoft.Cci.Extensions;
using Microsoft.Cci.Extensions.CSharp;

namespace Microsoft.Cci.Differs.Rules
{
    [ExportDifferenceRule]
    internal class CannotMakeNonVirtual : DifferenceRule
    {
        public override DifferenceType Diff(IDifferences differences, ITypeDefinitionMember impl, ITypeDefinitionMember contract)
        {
            if (impl == null || contract == null)
                return DifferenceType.Unknown;

            bool isImplInhertiable = IsInheritable(impl);
            bool isContractInheritiable = IsInheritable(contract);

            /*
            //@todo: Move to a separate rule that's only run in "strict" mode. 
            if (isImplInhertiable && !isContractInheritiable)
            {
                // This is separate because it can be noisy and is generally allowed as long as it is reviewed properly.
                differences.AddIncompatibleDifference("CannotMakeMemberVirtual",
                    "Member '{0}' is virtual in the implementation but non-virtual in the contract.",
                    impl.FullName());

                return DifferenceType.Changed;
            }
            */

            if (isContractInheritiable && !isImplInhertiable)
            {
                differences.AddIncompatibleDifference("CannotMakeMemberNonVirtual",
                    "Member '{0}' is non-virtual in the implementation but is virtual in the contract.",
                    impl.FullName());

                return DifferenceType.Changed;
            }

            return DifferenceType.Unknown;
        }

        private bool IsInheritable(ITypeDefinitionMember member)
        {
            if (!member.IsVirtual())
                return false;

            // member virtual final newslot == implicit interface implementation and is not inheritable
            if (member.IsSealed() && member.IsNewSlot())
                return false;

            return true;
        }

    }
}
