using System;
using System.IO;
using System.IO.Abstractions;
using System.Xml.Serialization;
using EawXBuild.Core;

namespace EawXBuild.Configuration.v1
{
    public class BuildConfigParser
    {
        private readonly IFileSystem _fileSystem;
        private readonly IBuildComponentFactory _factory;

        public BuildConfigParser(IFileSystem fileSystem, IBuildComponentFactory factory)
        {
            _fileSystem = fileSystem;
            _factory = factory;
        }

        public IProject[] Parse(string filePath)
        {
            var serializer = new XmlSerializer(typeof(BuildConfigurationType));
            IProject[] projects;
            using (var stream = _fileSystem.File.Open(filePath, FileMode.Open))
            {
                var buildConfig = serializer.Deserialize(stream) as BuildConfigurationType;
                projects = new IProject[buildConfig.Projects.Count];
                projects[0] = GetProjectFromConfig(buildConfig);
            }

            return projects;
        }

        private IProject GetProjectFromConfig(BuildConfigurationType buildConfig)
        {
            var buildConfigProject = buildConfig?.Projects[0];
            var project = _factory.MakeProject();
            project.Name = buildConfigProject.Name;

            AddJobToProject(buildConfigProject, project);

            return project;
        }

        private void AddJobToProject(ProjectType buildConfigProject, IProject project)
        {
            if (buildConfigProject.Jobs.Count == 0) return;
            var buildConfigJob = buildConfigProject.Jobs[0];
            var job = _factory.MakeJob(buildConfigJob.Name);
            project.AddJob(job);
        }
    }
}