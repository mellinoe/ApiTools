namespace Microsoft.Cci.Differs
{
    // These enums are priority so be careful about changing the order see ElementDiffer.Diff for the reason.
    public enum DifferenceType
    {
        Unknown,
        Unchanged,
        Added,
        Removed,
        Changed,
    }
}
