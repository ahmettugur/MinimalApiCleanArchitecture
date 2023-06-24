namespace MinimalApiCleanArchitecture.Application.IntegrationTests;

using static Testing;

public class BaseTestFixture
{
    public BaseTestFixture()
    {
        ResetState().GetAwaiter().GetResult();
        RunBeforeAnyTests();
    }
}