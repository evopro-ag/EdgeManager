using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using EdgeManager.Interfaces.Extensions;
using EdgeManager.Interfaces.Services;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Shouldly;

namespace EdgeManager.Tests
{
    public class DeviceMonitoringExtensionTests
    {
        [Test]
        public void MessagesAreGroupedByEvent()
        {
            //arrange
            var simulatedFeed = new List<string>()
            {
                "event:",
                "  component: ''",
                "  interface: ''",
                "  module: api",
                "  origin: MachineX",
                "  payload:",
                "    ContentType: ParameterValueContent",
                "    MachineName: MachineX",
                "    Payload:",
                "      Parameters:",
                "      - Number: 0",
                "        Value: 0.7753",
                "      - Number: 1",
                "        Value: 0.7719",
                "      - Number: 2",
                "        Value: 0.0",
                "      Position: 109.0",
                "    UUID: 31045594-c58d-4b26-b4be-b601576fa342",
                "    v: 1",
                "",
                "event:",
                "  component: ''",
                "  interface: ''",
                "  module: api",
                "  origin: MachineX",
                "  payload:",
                "    ContentType: DeviceStatusUpdate",
                "    Device: StationX",
                "    MachineName: MachineX",
                "    Payload:",
                "      Parameters:",
                "      - Name: ChangedValue",
                "        NewValue: 3.03",
                "        OldValue: 2.902",
                "        Unit: mm",
                "      Time: '2020-12-04T10:55:26.1516942+01:00'",
                "    UUID: 3ctze9d4-3ac8-48a6-bb26-8cb1f0e46905",
                "    v: 1",
                "",
                "event:",
            };
            
            var simulatedSubject = new Subject<string>();
            
            // // create a moq for ICommandHandler
            var commandHandlerMock = new Mock<ICommandHandler>();
            commandHandlerMock.Setup(handler => handler.OutputLines)
                .Returns(simulatedSubject)
                ;
            
            
            //act

            var retrievedMessages = new List<string>();
            var subscription = commandHandlerMock
                                                .Object
                                                .GetTelemetryMessages()
                                                .Subscribe(o => retrievedMessages.Add(o));
            // // Add some lines like in azure cli
            foreach (var line in simulatedFeed)
            {
                simulatedSubject.OnNext(line);
            }
            
            
            //assert

            // Task.Delay(500).Wait();
            
            var event1 = "  component: ''\n  interface: ''\n  module: api\n  origin: MachineX\n  payload:\n    ContentType: ParameterValueContent\n    MachineName: MachineX\n    Payload:\n      Parameters:\n      - Number: 0\n        Value: 0.7753\n      - Number: 1\n        Value: 0.7719\n      - Number: 2\n        Value: 0.0\n      Position: 109.0\n    UUID: 31045594-c58d-4b26-b4be-b601576fa342\n    v: 1";
            var event2 = "  component: ''\n  interface: ''\n  module: api\n  origin: MachineX\n  payload:\n    ContentType: DeviceStatusUpdate\n    Device: StationX\n    MachineName: MachineX\n    Payload:\n      Parameters:\n      - Name: ChangedValue\n        NewValue: 3.03\n        OldValue: 2.902\n        Unit: mm\n      Time: '2020-12-04T10:55:26.1516942+01:00'\n    UUID: 3ctze9d4-3ac8-48a6-bb26-8cb1f0e46905\n    v: 1";
            
            retrievedMessages.ShouldContain(event1);
            retrievedMessages.ShouldContain(event2);
        }
        
        [Test]
        public void MessagesAreGroupedByEventInJson()
        {
            //arrange
            var simulatedFeed = new List<string>()
            {
                "{",
                "    \"event\": {",
                "        \"origin\": \"GP630\",",
                "        \"module\": \"coilmachine\",",
                "        \"interface\": \"\",",
                "        \"component\": \"\",",
                "        \"payload\": {",
                "            \"ContentType\": \"DeviceAlive\",",
                "            \"EdgeSendTime\": \"2020-12-07T18:37:13.3699542Z\",",
                "            \"v\": 1,",
                "            \"UUID\": \"8b9c1b02-77c3-402c-96ae-36ee42ffc377\",",
                "            \"MachineName\": \"GP630\",",
                "            \"Payload\": {",
                "                \"DeviceName\": \"IoTEdge.coilmachine\"",
                "            }",
                "        }",
                "    }",
                "}",
                "{",
                "    \"event\": {",
                "        \"origin\": \"GP630\",",
                "        \"module\": \"systemstate\",",
                "        \"interface\": \"\",",
                "        \"component\": \"\",",
                "        \"payload\": {",
                "            \"ContentType\": \"DeviceAlive\",",
                "            \"EdgeSendTime\": \"2020-12-07T18:37:14.3267733Z\",",
                "            \"v\": 1,",
                "            \"UUID\": \"e514fcb6-3cf0-42c8-afa6-14c2dc216e3a\",",
                "            \"MachineName\": \"GP630\",",
                "            \"Payload\": {",
                "                \"DeviceName\": \"IoTEdge.systemstate\"",
                "            }",
                "        }",
                "    }",
            };
            
            var simulatedSubject = new Subject<string>();
            
            // // create a moq for ICommandHandler
            var commandHandlerMock = new Mock<ICommandHandler>();
            commandHandlerMock.Setup(handler => handler.OutputLines)
                .Returns(simulatedSubject)
                ;
            
            
            //act

            var retrievedMessages = new List<JObject>();
            var subscription = commandHandlerMock
                                                .Object
                                                .GetTelemetryMessagesInJsonFormat()
                                                .Subscribe(o => retrievedMessages.Add(o));
            // // Add some lines like in azure cli
            foreach (var line in simulatedFeed)
            {
                simulatedSubject.OnNext(line);
            }
            
            
            //assert

            // Task.Delay(500).Wait();

            var event1 = "{\"event\":{\"origin\":\"GP630\",\"module\":\"coilmachine\",\"interface\":\"\",\"component\":\"\",\"payload\":{\"ContentType\":\"DeviceAlive\",\"EdgeSendTime\":\"2020-12-07T18:37:13.3699542Z\",\"v\":1,\"UUID\":\"8b9c1b02-77c3-402c-96ae-36ee42ffc377\",\"MachineName\":\"GP630\",\"Payload\":{\"DeviceName\":\"IoTEdge.coilmachine\"}}}}";
            var event2 = "{\"event\":{\"origin\":\"GP630\",\"module\":\"systemstate\",\"interface\":\"\",\"component\":\"\",\"payload\":{\"ContentType\":\"DeviceAlive\",\"EdgeSendTime\":\"2020-12-07T18:37:14.3267733Z\",\"v\":1,\"UUID\":\"e514fcb6-3cf0-42c8-afa6-14c2dc216e3a\",\"MachineName\":\"GP630\",\"Payload\":{\"DeviceName\":\"IoTEdge.systemstate\"}}}}";
            
            retrievedMessages.Single(o => o["event"]["UUID"] == JsonConvert.DeserializeObject<JObject>(event1)["event"]["UUID"]).ShouldNotBeNull();
            retrievedMessages.Single(o => o["event"]["UUID"] == JsonConvert.DeserializeObject<JObject>(event2)["event"]["UUID"]).ShouldNotBeNull();
        }
    }
}