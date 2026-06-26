using SmartMemoryManager.Infrastructure.Cleanup;

namespace SmartMemoryManager.Tests;

[TestClass]
public sealed class CleanupTaskTests
{
    [TestMethod]
    public async Task AppTempCleanupTask_DeletesOnlyOldFilesInConfiguredDirectory()
    {
        var directory = Directory.CreateTempSubdirectory("smm-cleanup-");
        try
        {
            var oldFile = Path.Combine(directory.FullName, "old.tmp");
            var newFile = Path.Combine(directory.FullName, "new.tmp");
            await File.WriteAllTextAsync(oldFile, "old");
            await File.WriteAllTextAsync(newFile, "new");
            File.SetLastWriteTimeUtc(oldFile, DateTime.UtcNow.AddDays(-1));

            var task = new AppTempCleanupTask(new CleanupOptions(directory.FullName, directory.FullName));
            var recovered = await task.ExecuteAsync();

            Assert.IsTrue(recovered > 0);
            Assert.IsFalse(File.Exists(oldFile));
            Assert.IsTrue(File.Exists(newFile));
        }
        finally
        {
            directory.Delete(recursive: true);
        }
    }
}
