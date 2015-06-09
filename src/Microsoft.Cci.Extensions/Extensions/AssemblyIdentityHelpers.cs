using System;
using System.Globalization;
using System.Linq;

namespace Microsoft.Cci.Extensions
{
    public static class AssemblyIdentityHelpers
    {
        public static string Format(this AssemblyIdentity assemblyIdentity)
        {
            var name = new System.Reflection.AssemblyName();
            name.Name = assemblyIdentity.Name.Value;
            name.CultureInfo = new CultureInfo(assemblyIdentity.Culture);
            name.Version = assemblyIdentity.Version;
            name.SetPublicKeyToken(assemblyIdentity.PublicKeyToken.ToArray());
            name.CodeBase = assemblyIdentity.Location;
            return name.ToString();
        }

        public static AssemblyIdentity Parse(INameTable nameTable, string formattedName)
        {
            var name = new System.Reflection.AssemblyName(formattedName);
            return new AssemblyIdentity(nameTable.GetNameFor(name.Name),
                                        name.CultureInfo.Name,
                                        name.Version,
                                        name.GetPublicKeyToken(),
                                        name.CodeBase);
        }
    }
}