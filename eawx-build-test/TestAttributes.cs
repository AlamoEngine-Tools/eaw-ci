using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EawXBuildTest
{
    public class PlatformSpecificTestMethod : TestMethodAttribute
    {
        public PlatformSpecificTestMethod(params string[] platforms)
        {
            Platforms = platforms.Select(platformName => OSPlatform.Create(platformName.ToUpper()));
        }

        public IEnumerable<OSPlatform> Platforms { get; }

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            bool platformMatches = Platforms.Any(RuntimeInformation.IsOSPlatform);
            return !platformMatches
                ? new[] {new TestResult {Outcome = UnitTestOutcome.Inconclusive}}
                : base.Execute(testMethod);
        }
    }

    public class TestMethodWithRequiredEnvironmentVariable : TestMethodAttribute
    {
        private readonly string _requiredValue;
        private readonly string _variableName;

        public TestMethodWithRequiredEnvironmentVariable(string variableName, string requiredValue)
        {
            _variableName = variableName;
            _requiredValue = requiredValue;
        }

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            string? variable = Environment.GetEnvironmentVariable(_variableName);

            return variable == null || !variable.Equals(_requiredValue)
                ? new[] {new TestResult {Outcome = UnitTestOutcome.Inconclusive}}
                : base.Execute(testMethod);
        }
    }
}