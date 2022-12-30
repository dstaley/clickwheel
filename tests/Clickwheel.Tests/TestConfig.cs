using System.IO;
using System.Runtime.CompilerServices;
using VerifyTests;

namespace Clickwheel.Tests
{
    public static class TestConfig
    {
        [ModuleInitializer]
        public static void Init()
        {
            VerifierSettings.DerivePathInfo(
                (sourceFile, projectDirectory, type, method) =>
                    new(Path.Combine(projectDirectory, "Fixtures", "Snapshots"), type.Name, method.Name)
            );
        }
    }
}