using System;
using System.IO;

namespace MsBuilderific.Contracts.Extensions
{
    /// <summary>
    /// Extension methods to standard the <see cref="VisualStudioProject"/> classes.
    /// </summary>
    public static class VisualStudioProjectExtensions
    {
        public static string GetRelativeFilePath(this VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            if (project == null)
                throw new ArgumentNullException("project", "The project cannot be null");

            if (!String.IsNullOrEmpty(project.Path))
            {
                var folder = GetRelativeFolderPath(project, coreOptions);
                if (!String.IsNullOrEmpty(folder))
                    return Path.Combine(folder, Path.GetFileName(project.Path));

                return Path.GetFileName(project.Path);
            }

            throw new ArgumentException("The project path is empty or null");
        }

        public static string GetRelativeFolderPath(this VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            if (project == null)
                throw new ArgumentNullException("project", "The project cannot be null");

            if (coreOptions == null)
                throw new ArgumentNullException("coreOptions", "The coreOptions are mandatory to get the relative file path");

            var folder = Directory.GetParent(project.Path).FullName;

            if (!string.IsNullOrEmpty(coreOptions.RelativeToPath))
            {
                var relativePath = Directory.GetParent(project.Path).FullName.Replace(coreOptions.RelativeToPath, "");
                while (relativePath.StartsWith("\\"))
                    relativePath = relativePath.Substring(1);
                folder = relativePath;
            }

            return folder;
        }
    }
}
