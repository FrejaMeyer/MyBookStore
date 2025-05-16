using System;

namespace Frontend.Services
{
    public class CustomerSessionService
    {
        private string CustomerId;

        public CustomerSessionService()
        {
            // Gemmes kun i hukommelsen – kan udvides til localStorage senere
            CustomerId = Guid.NewGuid().ToString();
        }

        public string GetCustomerId()
        {
            return CustomerId;
        }
    }
}
