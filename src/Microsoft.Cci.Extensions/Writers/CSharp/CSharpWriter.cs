﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Cci.Extensions;
using Microsoft.Cci.Extensions.CSharp;
using Microsoft.Cci.Filters;
using Microsoft.Cci.Traversers;
using Microsoft.Cci.Writers.CSharp;
using Microsoft.Cci.Writers.Syntax;

namespace Microsoft.Cci.Writers
{
    public class CSharpWriter : SimpleTypeMemberTraverser, ICciWriter
    {
        private readonly ISyntaxWriter _syntaxWriter;
        private readonly IStyleSyntaxWriter _styleWriter;
        private readonly CSDeclarationWriter _declarationWriter;
        private readonly bool _writeAssemblyAttributes;
        private bool _firstMemberGroup;

        public CSharpWriter(ISyntaxWriter writer, ICciFilter filter, bool apiOnly, bool writeAssemblyAttributes = false)
            : base(filter)
        {
            _syntaxWriter = writer;
            _styleWriter = writer as IStyleSyntaxWriter;
            _declarationWriter = new CSDeclarationWriter(_syntaxWriter, filter, !apiOnly);
            _writeAssemblyAttributes = writeAssemblyAttributes;
        }

        public ISyntaxWriter SyntaxWriter { get { return _syntaxWriter; } }

        public ICciDeclarationWriter DeclarationWriter { get { return _declarationWriter; } }

        public bool IncludeSpaceBetweenMemberGroups { get; set; }

        public bool IncludeMemberGroupHeadings { get; set; }

        public bool HighlightBaseMembers { get; set; }

        public bool HighlightInterfaceMembers { get; set; }

        public bool IncludeGlobalPrefixForCompilation 
        {
            get { return _declarationWriter.ForCompilationIncludeGlobalPrefix; }
            set { _declarationWriter.ForCompilationIncludeGlobalPrefix = value; } 
        }

        public void WriteAssemblies(IEnumerable<IAssembly> assemblies)
        {
            foreach (var assembly in assemblies)
                Visit(assembly);
        }

        public override void Visit(IAssembly assembly)
        {
            if (_writeAssemblyAttributes)
            {
                _declarationWriter.WriteDeclaration(assembly);
            }

            base.Visit(assembly);
        }

        public override void Visit(INamespaceDefinition ns)
        {
            _declarationWriter.WriteDeclaration(ns);

            using (_syntaxWriter.StartBraceBlock())
            {
                base.Visit(ns);
            }

            _syntaxWriter.WriteLine();
        }

        public override void Visit(IEnumerable<ITypeDefinition> types)
        {
            WriteMemberGroupHeader(types.FirstOrDefault(Filter.Include) as ITypeDefinitionMember);
            base.Visit(types);
        }

        public override void Visit(ITypeDefinition type)
        {
            _declarationWriter.WriteDeclaration(type);

            if (!type.IsDelegate)
            {
                using (_syntaxWriter.StartBraceBlock())
                {
                    // If we have no constructors then output a private one this
                    // prevents the C# compiler from creating a default public one.
                    var constructors = type.Methods.Where(m => m.IsConstructor && Filter.Include(m));
                    if (!type.IsStatic && !constructors.Any())
                    {
                        // HACK... this will likely not work for any thing other than CSDeclarationWriter
                        _declarationWriter.WriteDeclaration(CSDeclarationWriter.GetDummyConstructor(type));
                        _syntaxWriter.WriteLine();
                    }
                    _firstMemberGroup = true;
                    base.Visit(type);
                }
            }
            _syntaxWriter.WriteLine();
        }

        public override void Visit(IEnumerable<ITypeDefinitionMember> members)
        {
            WriteMemberGroupHeader(members.FirstOrDefault(Filter.Include));
            base.Visit(members);
        }

        public override void Visit(ITypeDefinitionMember member)
        {
            IDisposable style = null;

            if (_styleWriter != null)
            {
                // Favor overrides over interface implemenations (i.e. consider override Dispose() as an override and not an interface implementation)
                if (this.HighlightBaseMembers && member.IsOverride())
                    style = _styleWriter.StartStyle(SyntaxStyle.InheritedMember);
                else if (this.HighlightInterfaceMembers && member.IsInterfaceImplementation())
                    style = _styleWriter.StartStyle(SyntaxStyle.InterfaceMember);
            }

            _declarationWriter.WriteDeclaration(member);

            if (style != null)
                style.Dispose();

            _syntaxWriter.WriteLine();
            base.Visit(member);
        }

        private void WriteMemberGroupHeader(ITypeDefinitionMember member)
        {
            if (IncludeMemberGroupHeadings || IncludeSpaceBetweenMemberGroups)
            {
                string header = CSharpWriter.MemberGroupHeading(member);

                if (header != null)
                {
                    if (IncludeSpaceBetweenMemberGroups)
                    {
                        if (!_firstMemberGroup)
                            _syntaxWriter.WriteLine(true);
                        _firstMemberGroup = false;
                    }

                    if (IncludeMemberGroupHeadings)
                    {
                        IDisposable dispose = null;
                        if (_styleWriter != null)
                            dispose = _styleWriter.StartStyle(SyntaxStyle.Comment);
                        
                        _syntaxWriter.Write("// {0}", header);

                        if (dispose != null)
                            dispose.Dispose();
                        _syntaxWriter.WriteLine();
                    }
                }
            }
        }

        public static string MemberGroupHeading(ITypeDefinitionMember member)
        {
            if (member == null)
                return null;

            IMethodDefinition method = member as IMethodDefinition;
            if (method != null)
            {
                if (method.IsConstructor)
                    return "Constructors";

                return "Methods";
            }

            IFieldDefinition field = member as IFieldDefinition;
            if (field != null)
                return "Fields";

            IPropertyDefinition property = member as IPropertyDefinition;
            if (property != null)
                return "Properties";

            IEventDefinition evnt = member as IEventDefinition;
            if (evnt != null)
                return "Events";

            INestedTypeDefinition nType = member as INestedTypeDefinition;
            if (nType != null)
                return "Nested Types";

            return null;
        }
    }
}
