using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.Cci.Extensions;
using Microsoft.Cci.Extensions.CSharp;
using Microsoft.Cci.Mappings;

namespace Microsoft.Cci.Differs.Rules
{
    [ExportDifferenceRule]
    internal class MembersMustExist : DifferenceRule
    {
        [Import]
        private IEqualityComparer<ITypeReference> _typeComparer = null;

        public override DifferenceType Diff(IDifferences differences, MemberMapping mapping)
        {
            ITypeDefinitionMember implMember = mapping[0];
            ITypeDefinitionMember contractMember = mapping[1];

            if (!(implMember == null && contractMember != null))
                return DifferenceType.Unknown;

            // Nested types are handled separately.
            // @TODO: Events and Properties - should we consider these too (or rely on the fact that dropping one of these will also drop their accessors.) 
            if (!(contractMember is IMethodDefinition || contractMember is IFieldDefinition))
                return DifferenceType.Unknown;  

            ITypeDefinition contractType = mapping.ContainingType[0];
            if (contractType != null)
            {
                IMethodDefinition contractMethod = contractMember as IMethodDefinition;
                if (contractMethod != null)
                {
                    // It is valid to have an explicit interface implementation in the contract and an implicit one in the implementation.
                    if (contractMethod.IsExplicitInterfaceMethod() &&
                        FindMatchingImplicit(contractType, contractMethod) != null)
                        return DifferenceType.Unknown;

                    // It is valid to promote a member from a base type up so check to see if it member exits on a base type.
                    if (FindMatchingBase(contractType, contractMethod) != null)
                        return DifferenceType.Unknown;
                }
            }

            differences.AddIncompatibleDifference(this,
                "Member '{0}' does not exist in the implementation but it does exist in the contract.", contractMember.FullName());

            return DifferenceType.Added;
        }

        private IMethodDefinition FindMatchingImplicit(ITypeDefinition type, IMethodDefinition explicitMethod)
        {
            if (type == null)
                return null;

            Contract.Assert(explicitMethod.IsExplicitInterfaceMethod());

            string name = explicitMethod.GetNameWithoutExplicitType();

            foreach (IMethodDefinition method in type.Methods)
            {
                if (method.Name.Value != name) continue;

                if (method.IsExplicitInterfaceMethod()) continue;

                if (ParameterTypesAreEqual(method, explicitMethod))
                {
                    if (!method.IsGeneric && !explicitMethod.IsGeneric)
                        return method;

                    if (method.GenericParameterCount == explicitMethod.GenericParameterCount)
                        return method;
                }
            }

            return null;
        }

        private IMethodDefinition FindMatchingBase(ITypeDefinition type, IMethodDefinition method)
        {
            if (type == null || method.IsConstructor)
                return null;

            foreach (var baseType in type.GetAllBaseTypes())
            {
                foreach (IMethodDefinition baseMethod in baseType.Methods)
                {
                    if (method.Name.Value != baseMethod.Name.Value) continue;

                    if (ParameterTypesAreEqual(method, baseMethod))
                    {
                        if (!method.IsGeneric && !baseMethod.IsGeneric)
                            return method;

                        if (method.GenericParameterCount == baseMethod.GenericParameterCount)
                            return method;
                    }
                }
            }
            return null;
        }

        private bool ParameterTypesAreEqual(IMethodDefinition implMethod, IMethodDefinition contractMethod)
        {
            IParameterDefinition[] params1 = implMethod.Parameters.ToArray();
            IParameterDefinition[] params2 = contractMethod.Parameters.ToArray();

            if (params1.Length != params2.Length)
                return false;

            for (int i = 0; i < params1.Length; i++)
            {
                IParameterDefinition param1 = params1[i];
                IParameterDefinition param2 = params2[i];

                if (!_typeComparer.Equals(param1.Type, param2.Type))
                    return false;
            }

            return true;
        }
    }
}
