namespace MinimalApiCleanArchitecture.Application.IntegrationTests;

using static Testing;

public class BaseTestFixture
{
    public BaseTestFixture()
    {
        RunBeforeAnyTests();
    }
    
    ~  BaseTestFixture()
    {
        ResetState().GetAwaiter().GetResult();
    }
}