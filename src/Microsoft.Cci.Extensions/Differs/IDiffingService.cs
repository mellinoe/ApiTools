using System.Collections.Generic;
using Microsoft.Cci.Writers.Syntax;

namespace Microsoft.Cci.Differs
{
    public interface IDiffingService
    {
        IEnumerable<SyntaxToken> GetTokenList(IDefinition definition);
    }
}
