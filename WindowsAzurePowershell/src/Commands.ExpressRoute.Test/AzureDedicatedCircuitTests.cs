﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

namespace Microsoft.WindowsAzure.Commands.ExpressRoute.Test
{
    using System;
    using System.Collections.Generic;
    using Commands.ExpressRoute;
    using VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Utilities.ExpressRoute;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Commands.Test.Utilities.Common;
    using Microsoft.WindowsAzure.Management.ExpressRoute;
    using Microsoft.WindowsAzure.Management.ExpressRoute.Models;

    [TestClass]
    public class AzureDedicatedCircuitTests : TestBase
    {
        private const string SubscriptionId = "foo";

        private static Mock<ExpressRouteManagementClient> InitExpressRouteManagementClient()
        {
            return
                (new Mock<ExpressRouteManagementClient>(
                    new CertificateCloudCredentials(SubscriptionId, new X509Certificate2(new byte[] { })),
                    new Uri("http://someValue")));
        }

        [TestMethod]
        public void NewAzureDedicatedCircuitSuccessful()
        {
            // Setup

            string circuitName = "TestCircuit";
            uint bandwidth = 10;
            string serviceProviderName = "TestProvider";
            string location = "us-west";
            string serviceKey = "aa28cd19-b10a-41ff-981b-53c6bbf15ead";

            MockCommandRuntime mockCommandRuntime = new MockCommandRuntime();
            Mock<ExpressRouteManagementClient> client = InitExpressRouteManagementClient();
            var dcMock = new Mock<IDedicatedCircuitOperations>();

            DedicatedCircuitGetResponse expected =
                new DedicatedCircuitGetResponse()
                {
                    DedicatedCircuit = new AzureDedicatedCircuit()
                    {
                        CircuitName = circuitName,
                        Bandwidth = bandwidth,
                        Location = location,
                        ServiceProviderName = serviceProviderName,
                        ServiceKey = serviceKey,
                        ServiceProviderProvisioningState = ProviderProvisioningState.NotProvisioned,
                        Status = DedicatedCircuitState.Enabled,
                    },
                    RequestId = "",
                    StatusCode = new HttpStatusCode()
                };
            var t = new Task<DedicatedCircuitGetResponse>(() => expected);
            t.Start();

            dcMock.Setup(f => f.NewAsync(It.Is<DedicatedCircuitNewParameters>(x => x.Bandwidth == bandwidth && x.CircuitName == circuitName && x.Location == location && x.ServiceProviderName == serviceProviderName), It.IsAny<CancellationToken>())).Returns((DedicatedCircuitNewParameters param, CancellationToken cancellation) => t);
            client.SetupGet(f => f.DedicatedCircuit).Returns(dcMock.Object);

            NewAzureDedicatedCircuitCommand cmdlet = new NewAzureDedicatedCircuitCommand()
            {
                CircuitName = circuitName,
                Bandwidth = bandwidth,
                Location = location,
                ServiceProviderName = serviceProviderName,
                CommandRuntime = mockCommandRuntime,
                ExpressRouteClient = new ExpressRouteClient(client.Object)
            };

            cmdlet.ExecuteCmdlet();

            // Assert
            AzureDedicatedCircuit actual = mockCommandRuntime.OutputPipeline[0] as AzureDedicatedCircuit;
            Assert.AreEqual<string>(expected.DedicatedCircuit.CircuitName, actual.CircuitName);
            Assert.AreEqual<uint>(expected.DedicatedCircuit.Bandwidth, actual.Bandwidth);
            Assert.AreEqual<string>(expected.DedicatedCircuit.Location, actual.Location);
            Assert.AreEqual<string>(expected.DedicatedCircuit.ServiceProviderName, actual.ServiceProviderName);
            Assert.AreEqual(expected.DedicatedCircuit.ServiceProviderProvisioningState, actual.ServiceProviderProvisioningState);
            Assert.AreEqual(expected.DedicatedCircuit.Status, actual.Status);
            Assert.AreEqual<string>(expected.DedicatedCircuit.ServiceKey, actual.ServiceKey);
        }

        [TestMethod]
        public void GetAzureDedicatedCircuitSuccessful()
        {
            // Setup

            string circuitName = "TestCircuit";
            uint bandwidth = 10;
            string serviceProviderName = "TestProvider";
            string location = "us-west";
            string serviceKey = "aa28cd19-b10a-41ff-981b-53c6bbf15ead";

            MockCommandRuntime mockCommandRuntime = new MockCommandRuntime();
            Mock<ExpressRouteManagementClient> client = InitExpressRouteManagementClient();
            var dcMock = new Mock<IDedicatedCircuitOperations>();

            DedicatedCircuitGetResponse expected =
                new DedicatedCircuitGetResponse()
                {
                    DedicatedCircuit = new AzureDedicatedCircuit()
                    {
                        CircuitName = circuitName,
                        Bandwidth = bandwidth,
                        Location = location,
                        ServiceProviderName = serviceProviderName,
                        ServiceKey = serviceKey,
                        ServiceProviderProvisioningState = ProviderProvisioningState.NotProvisioned,
                        Status = DedicatedCircuitState.Enabled,
                    },
                    RequestId = "",
                    StatusCode = new HttpStatusCode()
                };
            var t = new Task<DedicatedCircuitGetResponse>(() => expected);
            t.Start();

            dcMock.Setup(f => f.GetAsync(It.Is<string>(sKey => sKey == serviceKey), It.IsAny<CancellationToken>())).Returns((string sKey, CancellationToken cancellation) => t);
            client.SetupGet(f => f.DedicatedCircuit).Returns(dcMock.Object);

            GetAzureDedicatedCircuitCommand cmdlet = new GetAzureDedicatedCircuitCommand()
            {
                ServiceKey = serviceKey,
                CommandRuntime = mockCommandRuntime,
                ExpressRouteClient = new ExpressRouteClient(client.Object)
            };

            cmdlet.ExecuteCmdlet();

            // Assert
            AzureDedicatedCircuit actual = mockCommandRuntime.OutputPipeline[0] as AzureDedicatedCircuit;
            Assert.AreEqual<string>(expected.DedicatedCircuit.CircuitName, actual.CircuitName);
            Assert.AreEqual<uint>(expected.DedicatedCircuit.Bandwidth, actual.Bandwidth);
            Assert.AreEqual<string>(expected.DedicatedCircuit.Location, actual.Location);
            Assert.AreEqual<string>(expected.DedicatedCircuit.ServiceProviderName, actual.ServiceProviderName);
            Assert.AreEqual(expected.DedicatedCircuit.ServiceProviderProvisioningState, actual.ServiceProviderProvisioningState);
            Assert.AreEqual(expected.DedicatedCircuit.Status, actual.Status);
            Assert.AreEqual<string>(expected.DedicatedCircuit.ServiceKey, actual.ServiceKey);
        }

        [TestMethod]
        public void RemoveAzureDedicatedCircuitSuccessful()
        {
            string serviceKey = "aa28cd19-b10a-41ff-981b-53c6bbf15ead";
            ExpressRouteOperationStatusResponse expected =
                new ExpressRouteOperationStatusResponse()
                {
                    Status = ExpressRouteOperationStatus.Successful,
                    HttpStatusCode = HttpStatusCode.OK
                };
            
            MockCommandRuntime mockCommandRuntime = new MockCommandRuntime();
            Mock<ExpressRouteManagementClient> client = InitExpressRouteManagementClient();
            var dcMock = new Mock<IDedicatedCircuitOperations>();

            var t = new Task<ExpressRouteOperationStatusResponse>(() => expected);
            t.Start();

            dcMock.Setup(f => f.RemoveAsync(It.Is<string>(sKey => sKey == serviceKey), It.IsAny<CancellationToken>())).Returns((string sKey, CancellationToken cancellation) => t);
            client.SetupGet(f => f.DedicatedCircuit).Returns(dcMock.Object);

            RemoveAzureDedicatedCircuitCommand cmdlet = new RemoveAzureDedicatedCircuitCommand()
            {
                ServiceKey = serviceKey,
                CommandRuntime = mockCommandRuntime,
                ExpressRouteClient = new ExpressRouteClient(client.Object)
            };

            cmdlet.ExecuteCmdlet();

            Assert.IsTrue(mockCommandRuntime.VerboseStream[0].Contains(serviceKey));
        }

        [TestMethod]
        public void ListAzureDedicatedCircuitSuccessful()
        {
            // Setup

            string circuitName1 = "TestCircuit";
            uint bandwidth1 = 10;
            string serviceProviderName1 = "TestProvider";
            string location1 = "us-west";
            string serviceKey1 = "aa28cd19-b10a-41ff-981b-53c6bbf15ead";

            string circuitName2 = "TestCircuit2";
            uint bandwidth2 = 10;
            string serviceProviderName2 = "TestProvider";
            string location2 = "us-north";
            string serviceKey2 = "bc28cd19-b10a-41ff-981b-53c6bbf15ead";

            MockCommandRuntime mockCommandRuntime = new MockCommandRuntime();
            Mock<ExpressRouteManagementClient> client = InitExpressRouteManagementClient();
            var dcMock = new Mock<IDedicatedCircuitOperations>();

            List<AzureDedicatedCircuit> dedicatedCircuits = new List<AzureDedicatedCircuit>(){ 
                new AzureDedicatedCircuit(){ Bandwidth = bandwidth1, CircuitName = circuitName1, ServiceKey = serviceKey1, Location = location1, ServiceProviderName = serviceProviderName1, ServiceProviderProvisioningState = ProviderProvisioningState.NotProvisioned, Status = DedicatedCircuitState.Enabled}, 
                new AzureDedicatedCircuit(){ Bandwidth = bandwidth2, CircuitName = circuitName2, ServiceKey = serviceKey2, Location = location2, ServiceProviderName = serviceProviderName2, ServiceProviderProvisioningState = ProviderProvisioningState.Provisioned, Status = DedicatedCircuitState.Enabled}
            };
                 
            DedicatedCircuitListResponse expected =
                new DedicatedCircuitListResponse()
                {
                    DedicatedCircuits = dedicatedCircuits,
                    StatusCode = HttpStatusCode.OK                 
                };

            var t = new Task<DedicatedCircuitListResponse>(() => expected);
            t.Start();

            dcMock.Setup(f => f.ListAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken cancellation) => t);
            client.SetupGet(f => f.DedicatedCircuit).Returns(dcMock.Object);

             GetAzureDedicatedCircuitCommand cmdlet = new GetAzureDedicatedCircuitCommand()
            {
                CommandRuntime = mockCommandRuntime,
                ExpressRouteClient = new ExpressRouteClient(client.Object)
            };

            cmdlet.ExecuteCmdlet();

            // Assert
            IEnumerable<AzureDedicatedCircuit> actual =
                mockCommandRuntime.OutputPipeline[0] as IEnumerable<AzureDedicatedCircuit>;
            Assert.AreEqual(actual.ToArray().Count(), 2);
        }
    }
}