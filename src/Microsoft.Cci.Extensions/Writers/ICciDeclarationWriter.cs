
namespace Microsoft.Cci.Writers
{
    public interface ICciDeclarationWriter
    {
        void WriteDeclaration(IDefinition definition);
        void WriteAttribute(ICustomAttribute attribute);
    }
}
