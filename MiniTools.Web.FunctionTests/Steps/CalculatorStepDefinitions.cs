using TechTalk.SpecFlow;
using FluentAssertions;

namespace MiniTools.Web.FunctionTests.Steps
{
    [Binding]
    public sealed class CalculatorStepDefinitions
    {

        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        private readonly ScenarioContext _scenarioContext;

        public CalculatorStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given("the first number is (.*)")]
        public void GivenTheFirstNumberIs(int number)
        {
            _scenarioContext.Add("firstNumber", number);
        }

        [Given("the second number is (.*)")]
        public void GivenTheSecondNumberIs(int number)
        {
            _scenarioContext.Add("secondNumber", number);
        }

        [When("the two numbers are added")]
        public void WhenTheTwoNumbersAreAdded()
        {
            if (_scenarioContext.TryGetValue<int>("firstNumber", out int firstNumber)
                && _scenarioContext.TryGetValue<int>("secondNumber", out int secondNumber))
            {
                _scenarioContext.Add("result", firstNumber + secondNumber);
                return;
            }

            _scenarioContext.Pending();
        }

        [Then("the result should be (.*)")]
        public void ThenTheResultShouldBe(int result)
        {
            int resultNumber;
            if (_scenarioContext.TryGetValue<int>("result", out resultNumber))
            {
                resultNumber.Should().Be(120);
                return;
            }

            _scenarioContext.Pending();
        }
    }
}
