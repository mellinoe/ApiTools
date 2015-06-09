using Microsoft.Cci.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Microsoft.Cci.Extensions;

namespace Microsoft.Cci.Traversers
{
    public class DependencyTraverser : FilteredMetadataTraverser
    {
        private readonly Dictionary<IDefinition, HashSet<IReference>> _dependencies;
        private readonly Stack<IDefinition> _definitionStack;
        private readonly HashSet<IReference> _unresolvedDependencies;

        public DependencyTraverser(ICciFilter filter)
            : base(filter)
        {
            this._dependencies = new Dictionary<IDefinition, HashSet<IReference>>(new ReferenceEqualityComparer<IDefinition>());
            this._definitionStack = new Stack<IDefinition>();
            this._unresolvedDependencies = new HashSet<IReference>(new ReferenceEqualityComparer<IReference>());
        }

        public bool ComputeFullClosure { get; set; }

        public IDictionary<IDefinition, HashSet<IReference>> Dependencies { get { return _dependencies; } }

        public ISet<IReference> UnresolvedDependencies { get { return _unresolvedDependencies; } }

        public override void TraverseChildren(ITypeReference typeReference)
        {
            AddDependency(typeReference);
            base.TraverseChildren(typeReference);
        }

        public override void TraverseChildren(IMethodReference methodReference)
        {
            AddDependency(methodReference);
            base.TraverseChildren(methodReference);
        }

        public override void TraverseChildren(IFieldReference fieldReference)
        {
            AddDependency(fieldReference);
            base.TraverseChildren(fieldReference);
        }

        public override void TraverseChildren(ICustomAttribute customAttribute)
        {
            //base.Traverse(customAttribute.Type);
            base.TraverseChildren(customAttribute);
        }

        #region Definitions
        public override void TraverseChildren(IAssembly assembly)
        {
            this._definitionStack.Push(assembly);
            base.TraverseChildren(assembly);
            this._definitionStack.Pop();
        }

        public override void TraverseChildren(INamespaceTypeDefinition type)
        {
            this._definitionStack.Push(type);
            base.TraverseChildren(type);
            this._definitionStack.Pop();
        }

        public override void TraverseChildren(INestedTypeDefinition type)
        {
            this._definitionStack.Push(type);
            base.TraverseChildren(type);
            this._definitionStack.Pop();
        }

        public override void TraverseChildren(IMethodDefinition method)
        {
            this._definitionStack.Push(method);
            base.TraverseChildren(method);
            this._definitionStack.Pop();
        }

        public override void TraverseChildren(IEventDefinition eventDefinition)
        {
            this._definitionStack.Push(eventDefinition);
            base.TraverseChildren(eventDefinition);
            this._definitionStack.Pop();
        }

        public override void TraverseChildren(IPropertyDefinition propertyDefinition)
        {
            this._definitionStack.Push(propertyDefinition);
            base.TraverseChildren(propertyDefinition);
            this._definitionStack.Pop();
        }

        public override void TraverseChildren(IFieldDefinition fieldDefinition)
        {
            this._definitionStack.Push(fieldDefinition);

            if (fieldDefinition.Name.Value == "value__")
                Console.WriteLine("Why");
            base.TraverseChildren(fieldDefinition);
            this._definitionStack.Pop();
        }
        #endregion

        public override void TraverseChildren(IGenericParameter genericParameter)
        {
            base.TraverseChildren(genericParameter);
        }

        public override void TraverseChildren(IGenericMethodParameter genericMethodParameter)
        {
            base.TraverseChildren(genericMethodParameter);
        }

        private void AddDependency(ITypeReference type)
        {
            type = type.UnWrap();

            // We are not directly interested in generic instances or parameters, they will get broken down into 
            // their various pieces from the traversal and that is what we are interested in.
            if (type.IsGenericInstance() || type.IsGenericParameter())
                return;

            // We don't care about WindowsRuntime types
            if (type.IsWindowsRuntimeType())
                return;

            AddGeneralDependency(type);

            // Don't walk the full type for dependencies because a type dependency is only a reference to the type 
            // and we will already walk any particular method or field reference from it which is all we need.
        }

        private void AddDependency(IMethodReference method)
        {
            method = method.UnWrapMember<IMethodReference>();

            // We are not directly interested in generic instances, they will get broken down into 
            // their various pieces from the traversal and that is what we are interested in.
            if (method.IsGenericInstance())
                return;

            // We don't care about WindowsRuntime methods
            if (method.IsWindowsRuntimeMember())
                return;

            AddGeneralDependency(method);

            if (this.ComputeFullClosure)
            {
                IMethodDefinition methodDef = method.ResolvedMethod;

                if (methodDef is Dummy)
                {
                    this._unresolvedDependencies.Add(method);
                }
                else
                {
                    base.Traverse(methodDef);
                }
            }
        }

        private void AddDependency(IFieldReference field)
        {
            field = field.UnWrapMember<IFieldReference>();
            AddGeneralDependency(field);

            if (this.ComputeFullClosure)
            {
                IFieldDefinition fieldDef = field.ResolvedField;

                if (fieldDef is Dummy)
                {
                    this._unresolvedDependencies.Add(field);
                }
                else
                {
                    base.Traverse(fieldDef);
                }
            }
        }

        private void AddGeneralDependency(IReference reference)
        {
            Contract.Assert(this._definitionStack.Count != 0);

            IDefinition definition = this._definitionStack.Peek();

            HashSet<IReference> depends;
            if (!_dependencies.TryGetValue(definition, out depends))
            {
                depends = new HashSet<IReference>(new ReferenceEqualityComparer<IReference>());
                _dependencies.Add(definition, depends);
            }

            depends.Add(reference);
        }

        private class ReferenceEqualityComparer<T> : IEqualityComparer<T> where T : IReference
        {
            public bool Equals(T x, T y)
            {
                return string.Equals(x.UniqueId(), y.UniqueId());
            }

            public int GetHashCode(T obj)
            {
                return obj.UniqueId().GetHashCode();
            }
        }
    }
}
