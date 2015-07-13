using System.Diagnostics.Contracts;
using Microsoft.Cci.Extensions;
using Microsoft.Cci.Extensions.CSharp;

namespace Microsoft.Cci.Differs.Rules
{
    [ExportDifferenceRule]
    internal class EnumValuesMustMatch : DifferenceRule
    {
        public override DifferenceType Diff(IDifferences differences, ITypeDefinitionMember impl, ITypeDefinitionMember contract)
        {
            if (impl == null || contract == null)
                return DifferenceType.Unknown;

            if (!impl.ContainingTypeDefinition.IsEnum || !contract.ContainingTypeDefinition.IsEnum)
                return DifferenceType.Unknown;

            IFieldDefinition implField = impl as IFieldDefinition;
            IFieldDefinition contractField = contract as IFieldDefinition;

            Contract.Assert(implField != null || contractField != null);

            if (!object.Equals(implField.Constant.Value, contractField.Constant.Value))
            {
                ITypeReference implValType = impl.ContainingTypeDefinition.GetEnumType();
                ITypeReference contractValType = contract.ContainingTypeDefinition.GetEnumType();

                differences.AddIncompatibleDifference(this,
                    "Enum value '{0}' is ({1}){2} in the implementation but ({3}){4} in the contract.",
                    implField.FullName(), implValType.FullName(), implField.Constant.Value, 
                    contractValType.FullName(), contractField.Constant.Value);
                return DifferenceType.Changed;
            }

            return DifferenceType.Unknown;
        }
    }
}
