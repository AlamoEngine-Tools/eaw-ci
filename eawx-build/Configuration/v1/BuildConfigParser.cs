using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using EawXBuild.Core;
using EawXBuild.Exceptions;
using JetBrains.Annotations;

namespace EawXBuild.Configuration.v1
{
    public class BuildConfigParser
    {
        private const string XSD_RESOURCE_ID = "v1.eaw-ci.xsd";
        [NotNull] private readonly IFileSystem _fileSystem;
        [NotNull] private readonly IBuildComponentFactory _factory;

        public BuildConfigParser([NotNull] IFileSystem fileSystem, [NotNull] IBuildComponentFactory factory)
        {
            _fileSystem = fileSystem;
            _factory = factory;
        }

        public IProject[] Parse(string filePath)
        {
            IProject[] projects;
            XmlSerializer xmlDataSerializer = new XmlSerializer(typeof(BuildConfigurationType));
            var settings = GetXmlReaderSettings();
            using (Stream stream = _fileSystem.File.OpenRead(filePath))
            {
                using XmlReader reader = XmlReader.Create(stream, settings);
                BuildConfigurationType buildConfig = xmlDataSerializer.Deserialize(reader) as BuildConfigurationType;
                projects = new IProject[buildConfig.Projects.Length];
                projects[0] = GetProjectFromConfig(buildConfig);
            }

            return projects;
        }

        private IProject GetProjectFromConfig(BuildConfigurationType buildConfig)
        {
            var buildConfigProject = buildConfig?.Projects[0];
            var project = _factory.MakeProject();
            project.Name = buildConfigProject.Name;

            AddJobToProject(buildConfig, buildConfigProject, project);

            return project;
        }

        private void AddJobToProject(BuildConfigurationType buildConfig, ProjectType buildConfigProject,
            IProject project)
        {
            if (buildConfigProject.Jobs.Length == 0) return;
            var buildConfigJob = buildConfigProject.Jobs[0];
            var job = _factory.MakeJob(buildConfigJob.Name);

            AddTaskToJob(buildConfig, job, buildConfigJob);

            project.AddJob(job);
        }

        private void AddTaskToJob(BuildConfigurationType buildConfig, IJob job, JobType buildConfigJob)
        {
            var taskList = (TasksType) buildConfigJob.Item;
            if (taskList.Items == null) return;

            foreach (var taskListItem in taskList.Items)
            {
                Copy buildConfigTask = null;
                string taskName;
                if (taskListItem is TaskReferenceType taskRef)
                {
                    if (buildConfig.GlobalTasks == null) return;
                    foreach (var globalTask in buildConfig.GlobalTasks)
                        if (globalTask.Id.Equals(taskRef.ReferenceId))
                            buildConfigTask = (Copy) globalTask;

                    taskName = buildConfigTask.Id;
                }
                else
                {
                    buildConfigTask = (Copy) taskListItem;
                    taskName = buildConfigTask.Name;
                }

                var taskBuilder = _factory.Task(buildConfigTask.GetType().Name);

                var task = taskBuilder.With(nameof(buildConfigTask.Name), taskName)
                    .With(nameof(buildConfigTask.CopyFromPath), buildConfigTask.CopyFromPath)
                    .With(nameof(buildConfigTask.CopyToPath), buildConfigTask.CopyToPath)
                    .With(nameof(buildConfigTask.CopySubfolders), buildConfigTask.CopySubfolders)
                    .Build();

                job.AddTask(task);
            }
        }

        private static XmlReaderSettings GetXmlReaderSettings()
        {
            XmlSerializer xsdSchemaSerializer = new XmlSerializer(typeof(XmlSchema));
            XmlSchemaSet schemas = new XmlSchemaSet();
            XmlSchema schema;
            string res = Assembly.GetExecutingAssembly().GetManifestResourceNames()
                .Single(str => str.EndsWith(XSD_RESOURCE_ID));
            using (Stream xsdStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(res))
            {
                schema = xsdSchemaSerializer.Deserialize(xsdStream) as XmlSchema;
            }

            schemas.Add(schema);
            XmlReaderSettings settings = new XmlReaderSettings
            {
                Schemas = schemas,
                ValidationType = ValidationType.Schema,
                ValidationFlags = XmlSchemaValidationFlags.ProcessIdentityConstraints |
                                  XmlSchemaValidationFlags.ReportValidationWarnings |
                                  XmlSchemaValidationFlags.ProcessInlineSchema |
                                  XmlSchemaValidationFlags.ProcessSchemaLocation
            };
            settings.ValidationEventHandler += (sender, arguments) =>
                throw new XmlSchemaValidationException(arguments?.Message);
            return settings;
        }
    }
}