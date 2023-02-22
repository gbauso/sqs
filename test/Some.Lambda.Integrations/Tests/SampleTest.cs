using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Some.Lambda.Integrations.Tests
{
    internal class SampleTest : IntegrationTestBase
    {
        [TestCase(TestName = "Send message to SQS")]
        public async Task SendAMessageToYourQueue_And_AssertResultAfterWaiting()
        {
            //Act
            await _sqsClient.PublishMessageAsync("your message", typeof(string).FullName);

            var result = _waiter.Wait(() => {
                var dummyBool = false;
                for(int i = 0; i < 10; i++)
                {
                    if(i == 9) dummyBool = true;
                }

                return dummyBool;
            }, dummyBool => dummyBool == true);

            //Assert
            Assert.True(result);
        }
    }
}
