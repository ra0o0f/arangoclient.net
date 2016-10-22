using System.Resources;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if NETSTANDARD1_1
[assembly: AssemblyTitle("ArangoDB.Client Netstandard_1.1")]
#elif PORTABLE
[assembly: AssemblyTitle("ArangoDB.Client Portable_111")]
#else
[assembly: AssemblyTitle("ArangoDB.Client NET_4.5")]
#endif

[assembly: AssemblyDescription("ArangoDB .Net Client")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ArangoDB.Client")]
[assembly: AssemblyCopyright("Copyright ©  2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en")]

[assembly: InternalsVisibleTo("ArangoDB.Client.Test")]
[assembly: InternalsVisibleTo("ClientTesting")]

#if !PORTABLE
// Setting ComVisible to false makes the types in this assembly not visible 
// to COM componenets.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]
//The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("A03E3606-5617-481B-AE49-AFB52FA5C087")]
#endif

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.7.60")]
[assembly: AssemblyFileVersion("0.7.60")]
[assembly: CLSCompliant(true)]