using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using EawXBuild.Core;
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
            XmlSerializer xsdSchemaSerializer = new XmlSerializer(typeof(XmlSchema));
            XmlSerializer xmlDataSerializer = new XmlSerializer(typeof(BuildConfigurationType));
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

            AddJobToProject(buildConfigProject, project);

            return project;
        }

        private void AddJobToProject(ProjectType buildConfigProject, IProject project)
        {
            if (buildConfigProject.Jobs.Length == 0) return;
            var buildConfigJob = buildConfigProject.Jobs[0];
            var job = _factory.MakeJob(buildConfigJob.Name);
            project.AddJob(job);
        }
    }
}
