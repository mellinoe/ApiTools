using System;

namespace Microsoft.Cci.Writers
{
    public interface IReviewCommentWriter
    {
        void WriteReviewComment(string author, string text);
    }
}
