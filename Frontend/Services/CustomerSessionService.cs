using System;

namespace Frontend.Services
{
    public class CustomerSessionService
    {
        private string CustomerId;

        public CustomerSessionService()
        {
            CustomerId = Guid.NewGuid().ToString();
        }

        public string GetCustomerId()
        {
            return CustomerId;
        }
    }
}
