using System.Linq;
using System.Reflection;
using Guts.Client.Core;
using Guts.Client.Core.TestTools;
using Lingo.Domain.Pit;
using Lingo.Domain.Pit.Contracts;
using Lingo.TestTools;
using NUnit.Framework;

namespace Lingo.Domain.Tests
{
    public class BallTests : TestBase
    {
        private string _iBallHash;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _iBallHash = Solution.Current.GetFileHash(@"Lingo.Domain\Pit\Contracts\IBall.cs");
        }

        [MonitoredTest("Should Have A Constructor That Accepts A Type And A Value - Already Some Balls In The Pit")]
        public void _01_ShouldHaveAConstructorThatAcceptsATypeAndAValue()
        {
            AssertThatInterfaceIsNotChanged();
            BallType type = BallType.Blue;
            int value = RandomGenerator.Next(1, 75);
            IBall ball = ConstructBall(type, value);
            Assert.That(ball.Type, Is.EqualTo(type),
                "After construction the 'Type' property should return the value passed into the constructor.");
            Assert.That(ball.Value, Is.EqualTo(value),
                "After construction the 'Value' property should return the value passed into the constructor.");
        }

        [MonitoredTest("Value and type - Should be readonly")]

        public void _02_ValueAndType_ShouldBeReadOnly()
        {
            AssertThatInterfaceIsNotChanged();

            var ballType = typeof(Ball);

            Assert.That(ballType.IsAssignableTo(typeof(IBall)), Is.True, "'Ball' must implement the 'IBall' interface.");

            PropertyInfo valueProperty = ballType.GetProperties().FirstOrDefault(p => p.Name == nameof(IBall.Value));
            Assert.That(valueProperty, Is.Not.Null, "Cannot find a 'Value' property");
            Assert.That(valueProperty.SetMethod == null || valueProperty.SetMethod.IsPublic == false, Is.True,
                "It should not be possible to write a value in the 'Value' property from outside of the class.");

            PropertyInfo typeProperty = ballType.GetProperties().FirstOrDefault(p => p.Name == nameof(IBall.Type));
            Assert.That(typeProperty, Is.Not.Null, "Cannot find a 'Type' property");
            Assert.That(typeProperty.SetMethod == null || typeProperty.SetMethod.IsPublic == false, Is.True,
                "It should not be possible to write a value in the 'Type' property from outside of the class.");
        }

        private IBall ConstructBall(BallType type, int value)
        {
            var ballType = typeof(Ball);

            var allConstructors = ballType.GetConstructors();
            Assert.That(allConstructors.Length, Is.EqualTo(1), "There should be exactly one constructor. No more, no less.");

            ConstructorInfo constructor = allConstructors.First();
            var constructorParameters = constructor.GetParameters();
            Assert.That(constructorParameters.Length, Is.EqualTo(2), "The constructor should have 2 parameters. No more, no less.");
            var ballTypeParameter = constructorParameters.First();
            Assert.That(ballTypeParameter.ParameterType, Is.EqualTo(typeof(BallType)),
                $"The first parameter of the constructor should be of type {nameof(BallType)}.");
            var valueParameter = constructorParameters.ElementAt(1);
            Assert.That(valueParameter.ParameterType, Is.EqualTo(typeof(int)),
                "The second parameter of the constructor should be of type int.");
            Assert.That(valueParameter.DefaultValue, Is.EqualTo(0),
                "The second parameter of the constructor should have '0' as default value.");

            Ball ball = (Ball)constructor.Invoke(new object[] { type, value });


            IBall ballAsInterfaceType = ball as IBall;
            Assert.That(ballAsInterfaceType, Is.Not.Null, "'Ball' must implement 'IBall'.");

            return ballAsInterfaceType;
        }


        private void AssertThatInterfaceIsNotChanged()
        {
            Assert.That(_iBallHash, Is.EqualTo("4B-07-C4-87-4C-E0-5D-4A-12-A7-34-31-0B-F4-7A-17"),
                "The code of the IBall interface has changed. This is not allowed. Undo your changes in 'IBall.cs'");
        }
    }
}