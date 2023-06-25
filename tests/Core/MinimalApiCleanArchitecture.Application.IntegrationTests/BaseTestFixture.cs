namespace MinimalApiCleanArchitecture.Application.IntegrationTests;

using static Testing;

public class BaseTestFixture
{
    protected BaseTestFixture()
    {
        ResetState().GetAwaiter().GetResult();
        RunBeforeAnyTests();
    }
}