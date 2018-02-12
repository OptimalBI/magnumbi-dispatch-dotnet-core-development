# .Net Core - MagnumBI Dispatch Client Repository.

MagnumBI Dispatch manages microservice communication and interaction simply.   
It is easy to develop and integrate with your small to medium sized development teams.   
To see more about MagnumBI Dispatch and download the server [click here](https://github.com/OptimalBI/magnumbi-dispatch-server)   

## Requirements

.Net Core 1.x or 2.x (.Net standard 1.3 or higher)

## Getting started

Clone this project to start.  
 See the example below for more details.


## Examples

```csharp
MagnumBiDispatchClient client = new MagnumBiDispatchClient(
    serverAddress: "https://127.0.0.1:6883",
    accessToken: "test",
    secretToken: "token",
    verifySsl: false
);

// Check the client can get a connection to the server.
bool status = await client.CheckStatus();

if (!status) {
    Console.Error.WriteLine("Could not connect to server.");
    Console.Error.WriteLine(client.LastStatusCode);
    Console.Error.WriteLine(client.LastErrorMessage ?? "");
}
```

For more example see the MagnumBi.Dispatch.Client.Examples folder.