using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using EawXBuild.Core;

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

        public IEnumerable<IProject> Parse(string filePath)
        {
            IProject[] projects;
            XmlSerializer xmlDataSerializer = new XmlSerializer(typeof(BuildConfigurationType));
            var settings = GetXmlReaderSettings();
            using (Stream stream = _fileSystem.File.OpenRead(filePath))
            {
                using XmlReader reader = XmlReader.Create(stream, settings);
                BuildConfigurationType buildConfig = xmlDataSerializer.Deserialize(reader) as BuildConfigurationType;
                projects = new IProject[buildConfig.Projects.Length];
                for (int i = 0; i < buildConfig.Projects.Length; i++)
                {
                    var buildConfigProject = buildConfig.Projects[i];
                    projects[i] = GetProjectFromConfig(buildConfig, buildConfigProject);
                }
            }

            return projects;
        }

        private IProject GetProjectFromConfig(BuildConfigurationType buildConfig, ProjectType buildConfigProject)
        {
            var project = _factory.MakeProject();
            project.Name = buildConfigProject.Name;
            AddJobsToProject(buildConfig, buildConfigProject, project);

            return project;
        }

        private void AddJobsToProject(BuildConfigurationType buildConfig, ProjectType buildConfigProject,
            IProject project)
        {
            if (buildConfigProject.Jobs.Length == 0) return;
            foreach (JobType buildConfigJob in buildConfigProject.Jobs)
            {
                IJob job = _factory.MakeJob(buildConfigJob.Name);
                AddTasksToJob(buildConfig, job, buildConfigJob);
                project.AddJob(job);
            }
        }

        private void AddTasksToJob(BuildConfigurationType buildConfig, IJob job, JobType buildConfigJob)
        {
            var taskList = (TasksType)buildConfigJob.Item;
            if (taskList.Items == null) return;

            foreach (var taskListItem in taskList.Items)
            {
                object buildConfigTask = GetBuildConfigTaskFromTaskListItem(buildConfig, taskListItem);

                ITask task = MakeTask(buildConfigTask);
                job.AddTask(task);
            }
        }

        private static object GetBuildConfigTaskFromTaskListItem(BuildConfigurationType buildConfig, object taskListItem)
        {
            object buildConfigTask = taskListItem;
            if (taskListItem is TaskReferenceType taskRef)
                buildConfigTask = GetMatchingGlobalTask(buildConfig, taskRef);

            return buildConfigTask;
        }

        private static object GetMatchingGlobalTask(BuildConfigurationType buildConfig, TaskReferenceType taskRef)
        {
            object buildConfigTask = null;
            foreach (var globalTask in buildConfig.GlobalTasks)
            {
                if (!globalTask.Id.Equals(taskRef.ReferenceId)) continue;
                buildConfigTask = globalTask;
            }

            return buildConfigTask;
        }

        private ITask MakeTask(object buildConfigTask)
        {
            var taskBuilder = _factory.Task(buildConfigTask.GetType().Name);
            foreach (var prop in buildConfigTask.GetType().GetProperties())
                taskBuilder.With(prop.Name, prop.GetValue(buildConfigTask));

            ITask task = taskBuilder.Build();
            return task;
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