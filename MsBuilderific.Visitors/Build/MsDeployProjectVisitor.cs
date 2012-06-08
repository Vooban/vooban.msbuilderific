using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MsBuilderific.Contracts;
using MsBuilderific.Contracts.Extensions;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class MsDeployProjectVisitor : BuildOrderVisitor
    {
        private readonly MsDeployProjectVisitorOptions _options;

        public MsDeployProjectVisitor(MsDeployProjectVisitorOptions options)
        {
            _options=options;
        }

        public override bool ShallExecute(IMsBuilderificCoreOptions coreOptions)
        {
            return _options.GeneratePackagesOnBuild;
        }

        public override string VisitBuildExeProjectTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            var packageCommand = new StringBuilder();
            var folder = project.GetRelativeFolderPath(coreOptions);
            var filename = project.GetRelativeFilePath(coreOptions);

            var packagingCondition = string.Format("Condition=\"$(GenerateMsDeployPackages) AND Exists('{0}\\bin') \" ", folder);                

            packageCommand.AppendLine(string.Format("		<MakeDir Directories=\"$(MSBuildProjectDirectory)\\{0}\\bin\\Packages\\$(Configuration)\\\" {1} />", folder, packagingCondition));
            packageCommand.AppendLine(String.Format("		<Exec Command='\"$(MsDeployLocation)\" -verb:sync -source:contentPath=\"$(MSBuildProjectDirectory)\\{0}\" -dest:package=\"$(MSBuildProjectDirectory)\\{0}\\bin\\Packages\\$(Configuration)\\{1}.zip\" -skip:objectName=filePath,absolutePath=\".*\\.vb|cache.*\" -skip:objectName=dirPath,absolutePath=\"obj\" -skip:objectName=dirPath,absolutePath=\"_Resharper\" -skip:objectName=dirPath,absolutePath=\"Packages\" -skip:xpath=\"//dirPath[count(*)=0]\" -skip:objectName=dirPath,absolutePath=\"_PublishedWebsites\" ' {2} />", folder, Path.GetFileNameWithoutExtension(filename), packagingCondition));

            return packageCommand.ToString();
        }

        public override string VisitBuildWebProjectTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            var packageCommand = new StringBuilder();
            var folder = project.GetRelativeFolderPath(coreOptions);

            if (_options.Transforms != null && _options.Transforms.Count > 0)
            {
                _options.Transforms.ForEach(t =>
                                               {
                                                   packageCommand.AppendLine(string.Format("		<Delete Files=\"$(MSBuildProjectDirectory)\\{0}\\web.temp.{1}.config\" Condition=\"$(GenerateMsDeployPackages) AND Exists('{0}\\web.temp.{1}.config')\" />", folder, t));
                                                   packageCommand.AppendLine(string.Format("		<Copy SourceFiles=\"$(MSBuildProjectDirectory)\\{0}\\web.config\" DestinationFiles=\"$(MSBuildProjectDirectory)\\{0}\\web.temp.{1}.config\" Condition=\"$(GenerateMsDeployPackages) AND Exists('{0}\\web.{1}.config')\" />", folder, t));
                                                   packageCommand.AppendLine(string.Format("		<TransformXml Source=\"$(MSBuildProjectDirectory)\\{0}\\web.temp.{1}.config\" Transform=\"$(MSBuildProjectDirectory)\\{0}\\web.{1}.config\" Destination=\"$(MSBuildProjectDirectory)\\{0}\\web.transformed.{1}.config\" Condition=\"$(GenerateMsDeployPackages) AND Exists('{0}\\web.{1}.config')\" />", folder, t));
                  
                                                   packageCommand.AppendLine(GetMsDeployCommand(project, coreOptions, t, true));
                                               });
            }

            packageCommand.AppendLine(GetMsDeployCommand(project, coreOptions));

            return packageCommand.ToString();     
        }

        public override string VisitServiceTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {            
            return VisitBuildWebProjectTarget(project, coreOptions);
        }

        private string GetMsDeployCommand(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions, string configuration = "$(Configuration)", bool isTransform = false)
        {
            var packageCommand = new StringBuilder();
            var folder = project.GetRelativeFolderPath(coreOptions);
            var filename = project.GetRelativeFilePath(coreOptions);
            var regExParamEntry = string.Format(".+\\\\{0}", Regex.Escape(folder));

            string conditions ;
            if(isTransform)
                conditions = string.Format("Condition=\"$(GenerateMsDeployPackages) AND Exists('{0}\\web.{1}.config')\"", folder, configuration);
            else
                conditions = string.Format("Condition=\"$(GenerateMsDeployPackages) AND Exists('{0}\\bin')\"", folder);

            packageCommand.AppendLine(string.Format("		<MakeDir Directories=\"$(MSBuildProjectDirectory)\\{0}\\bin\\Packages\\{1}\\\" {2} />", folder, configuration, conditions));
            string skipWebConfig = null;
            if (isTransform)
            {
                var assemblyNameEscaped = Regex.Escape(string.Format("{0}\\{1}", project.AssemblyName, "web.config"));
                skipWebConfig = string.Format("-skip:objectname=filepath,absolutepath=\"{0}\" -replace:objectName=filepath,scopeAttributeName=path,match=web.transformed.{1}.config,replace=web.config", assemblyNameEscaped, configuration);
            }
            packageCommand.AppendLine(String.Format("		<Exec Command='\"$(MsDeployLocation)\" -verb:sync -source:iisApp=\"$(MSBuildProjectDirectory)\\{0}\" -dest:package=\"$(MSBuildProjectDirectory)\\{0}\\bin\\Packages\\{1}\\{2}.zip\" -declareParam:name=\"IIS Web Application Name\",value=\"{2}\",tags=IisApp,kind=ProviderPath,scope=IisApp,match=\"{3}\" {4} -skip:objectName=filePath,absolutePath=\".*\\.vb$|pdb$\" -skip:objectName=dirPath,absolutePath=\"obj|_Resharper|Packages|_PublishedWebsites\" ' {5} />", folder, configuration, Path.GetFileNameWithoutExtension(filename), regExParamEntry, skipWebConfig, conditions));

            return packageCommand.ToString();     
        }
    }
}
