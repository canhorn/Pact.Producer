using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using tests.XUnitHelpers;
using Xunit;
using Xunit.Abstractions;

namespace tests
{
    public class ProductTest
    {
        private string _serviceUri = "http://localhost:5000";
        private ITestOutputHelper _outputHelper { get; }

        public ProductTest(ITestOutputHelper output)
        {
            _outputHelper = output;
        }

        [Fact]
        public async Task Testing()
        {
            // Given
            var pactBrokerBaseUrl = Environment.GetEnvironmentVariable("PACT_BROKER_BASE_URL");
            var pactBrokerToken = Environment.GetEnvironmentVariable("PACT_BROKER_TOKEN");

            var branch = Environment.GetEnvironmentVariable("GIT_BRANCH");
            var providerVersion = Environment.GetEnvironmentVariable("GIT_COMMIT");
            var pactEnv = Environment.GetEnvironmentVariable("PACT_ENV");
            var publishVerificationResults = "true".Equals(
                Environment.GetEnvironmentVariable("PACT_BROKER_PUBLISH_VERIFICATION_RESULTS")
            );
            var config = new PactVerifierConfig
            {
                //NOTE: Setting a provider version is required for publishing verification results
                ProviderVersion = providerVersion,
                PublishVerificationResults = true, //PublishVerificationResults,
                // NOTE: We default to using a ConsoleOutput,
                // however xUnit 2 does not capture the console output,
                // so a custom outputter is required.
                Outputters = new List<IOutput> { new XUnitOutput(_outputHelper) },
                // Output verbose verification logs to the test output
                Verbose = true,
            };
            // When / Then
            IPactVerifier pactVerifier = new PactVerifier(config);
            pactVerifier
                .ServiceProvider("product-service", _serviceUri)
                .PactBroker(
                    pactBrokerBaseUrl, //"https://canhorn.pactflow.io",
                    uriOptions: new PactUriOptions(
                        pactBrokerToken
                    ),
                    providerVersionTags: new List<string> { branch, pactEnv }
                )
                // .HonoursPactWith("api-client")
                // .PactUri(@"..\..\..\..\..\pacts\api-client-product-service.json")
                .Verify();
        }
    }
}
