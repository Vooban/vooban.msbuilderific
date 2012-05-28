MsBuilderific
=============
MsBuilderific then find and detect every dependencies between the projects (either project reference or file reference) and produce a dependency graph to construct the build script.

The goal is to generate MsBuild script for building multiple projects that are specified in input. MsBuilderific then find and detect every dependencies between the projects (either project reference or file reference) and produce a dependency graph to construct the build script.

With this tool, you can easily create MSBuild script with the Clean, Rebuild and Build targets. MsBuilderific will automatically detect the dependencies between your project and create a MSBuild file for you, with either absolute path or relative path, you decide. You can also decide to generate specific MSBuild targets only for web projects, which you usually build more often to refresh their bin directory, if you use a common build folder where you get your dll from. Yes I know, Nuget is here and we shall all use it, but sometime we don't decide or can't decide to port all of our libraries to NuGet.

The library also makes use of the great QuickGraph project (http://quickgraph.codeplex.com) to create and navigate the projects' dependency graph. With QuickGraph's GraphML feature, we also offer the capability to ouput you projects dependecies as a GraphML file, which can be easily displayed using Gephi (http://gephi.org/).
Comments

Please provide feedback, comments and suggestions so that we can add more interesting features to this tool! We're counting on you guys!
How to invoke MsBuilderific

The project comes in the form a dll and can be easily integrated into you build server or any custom application. Using MsBuilderific is quite simple, here's how :

```csharp
// Supports only the vbproj
var finder = new ProjectDependencyFinder(true, false);

// Add exclusions
finder.AddExclusionPattern(@"Source\OldSolutionFolder");
finder.AddExclusionPattern("Common.Web.vbproj");

// Generate the dependency order and persist the graph
var buildOder = finder.GetDependencyOrder(@"C:\Source\", @"C:\Source\Build\mybuilddependencies.graphml");

// Generate a MsBuild file in C:\SourceBuild named mybuildfile.build and 
// copy built DLL to C:\Source\Binaries automatically in the build script
var generator = new MsBuildFileGenerator(@"C:\Source\Build\mybuildfile.build", "C:\Source\Binaries", true);
generator.WriteBuildScript(buildOder);
```

How to invoke the generated build file
--------------------------------------

To invoke the build once your build file is generated use the following :

```
msbuild myproject.build /t:rebuild
```

The following options can be specified to msbuild :
```
/target:targetName
The available targets are :
Clean : Deletes all ouput files in the project's obj and bin folder, and also delete any content in the deployment folder
Build : Build all the project in the right order
Rebuild : Cleans and then perform a build operation
CleanServices : Deletes all the web project output files in the project's obj and bin folder, and also delete any content in the deployment folder
BuildServices : Build all web related projects in the right order
RebuildServices : Cleans and then perform a build operation of web projects
/verbosity:level
{The available verbosity levels are q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic]}. /v is also acceptable. For exemple /v:m
/property:ContinueOnError=True/False
```

If set to true, allows you to continue building projects even if one of these project failed to compile
