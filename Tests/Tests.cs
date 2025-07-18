namespace Tests
{
    public class Tests
    {
        [Fact]
        public void TestShell()
        {
            Assert.True(CollinExecute.Shell.SystemCommand("echo Hello World", stream: false, treatStderrAsFailure: false));
        }
    }
}
