using System;
using System.IO;
using Domain.Contracts;
using Domain.Users;

namespace Domain.Languages
{
    public record PlatformId(Guid Value);

    public record EnvironmentId(Guid Value): IEntityId
    {
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Environment : Entity<EnvironmentId>
    {
        public Environment
        (EnvironmentId id, PlatformId platformId, string runTestsProcess, string runTestsCommand,
            string installationCommand, string installationProcess, string testsFileRelativePath,
            string sutFileRelativePath, Stream template, string? templateInstallationProcess,
            string? templateInstallationCommand) : base(id)
        {
            PlatformId = platformId;
            RunTestsProcess = runTestsProcess;
            RunTestsCommand = runTestsCommand;
            InstallationCommand = installationCommand;
            InstallationProcess = installationProcess;
            TestsFileRelativePath = testsFileRelativePath;
            SutFileRelativePath = sutFileRelativePath;
            Template = template;
            TemplateInstallationProcess = templateInstallationProcess;
            TemplateInstallationCommand = templateInstallationCommand;
        }

        public PlatformId PlatformId { get; set; }
        public string RunTestsProcess { get; set; }
        public string RunTestsCommand { get; set; }
        public string InstallationCommand { get; set; }
        public string InstallationProcess { get; set; }
        public string TestsFileRelativePath { get; set; }
        public string SutFileRelativePath { get; set; }
        public string? TemplateInstallationProcess { get; set; }
        public string? TemplateInstallationCommand { get; set; }
        public Stream Template { get; set; }
    }
}