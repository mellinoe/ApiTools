
namespace Microsoft.Cci.Writers.CSharp
{
    public partial class CSDeclarationWriter
    {
        public void WriteNamespaceDeclaration(INamespaceDefinition ns)
        {
            WriteKeyword("namespace");
            WriteIdentifier(TypeHelper.GetNamespaceName((IUnitNamespace)ns, NameFormattingOptions.None));
        }
    }
}
