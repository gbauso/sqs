
using Some.Lambda.Integrations.Clients;
using System.Threading.Tasks;

namespace Some.Lambda.Integrations
{
    public abstract class IntegrationTestBase
    {
        protected readonly SqsClient _sqsClient;
        protected readonly ActionWaiter _waiter;

        public IntegrationTestBase()
        {
            _sqsClient = new SqsClient();
            _waiter = new ActionWaiter();
        }


    }
}
