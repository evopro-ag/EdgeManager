using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using EdgeManager.Interfaces.Extensions;
using EdgeManager.Interfaces.Services;
using Moq;
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
    }
}