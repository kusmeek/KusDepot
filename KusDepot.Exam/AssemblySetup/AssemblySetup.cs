[SetUpFixture]
public class AssemblySetup
{
    [OneTimeSetUp]
    public void Calibrate()
    {
        KusDepot.Settings.NoExceptions = false;
    }
}