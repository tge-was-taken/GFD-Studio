using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle( "GFD Studio" )]
[assembly: AssemblyDescription("Editor for graphical assets used in recent Persona games")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyProduct( "GFD Studio" )]
[assembly: AssemblyCopyright("Copyright © 2018 TGE")]
[assembly: AssemblyTrademark("TGE")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("75d72552-d73f-47f9-bc26-a5c6be341901")]

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
[assembly: AssemblyVersion( "0.4.0.7" )]
