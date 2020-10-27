using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PaymentGatewayAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGatewayTests
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration env) : base(env)
        {

        }
    }
}
