// See https://aka.ms/new-console-template for more information
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using Azure.Messaging.EventGrid;

var eventGridEvent = new EventGridEvent(
    subject: "DeviceTelemetry",
    dataVersion: "1.0",
    eventType: "Microsoft.Devices.DeviceTelemetry",
    data: new BinaryData("{\"body\": [{\"temperature\": 25.5, \"humidity\": 50.0}, {\"temperature\": 26.2, \"humidity\": 50.0}], \"enqueuedTime\": \"2021-09-01T00:00:00Z\", \"Properties\": {\"deviceId\": \"device1\", \"areaId\": \"area1\", \"siteId\": \"site1\", \"organizationId\": \"org1\"}, \"SystemProperties\": { \"iothub-connection-device-id\": \"device1\", \"iothub-connection-auth-method\": \"sas\", \"iothub-connection-auth-generation-id\": \"1234\" }}")
);

var eventGridEventData = eventGridEvent.Data.ToObjectFromJson<JsonObject>();

var systemProperties = eventGridEventData["systemProperties"];
var properties = eventGridEventData["properties"] as JsonObject;

var deviceId = systemProperties?["iothub-connection-device-id"]?.ToString();
var areaId = properties?["areaId"]?.ToString();
var siteId = properties?["siteId"]?.ToString();
var organizationId = properties?["organizationId"]?.ToString();

var body = eventGridEventData["body"] as JsonArray;
if (body is not null) {
    foreach (JsonObject? message in body)
    {
        if (message is not null) {
            message.Add("DeviceId", deviceId);
            message.Add("AreaId", areaId);
            message.Add("SiteId", siteId);
            message.Add("OrganizationId", organizationId);
        }
    }
}

// Get the body as a JSON string
Console.WriteLine((string)JsonSerializer.Serialize(body));

// Get the properties as a Dictionary<string, object>
Console.WriteLine(properties?.ToDictionary(p => p.Key ?? string.Empty, p => (object?)p.Value));
