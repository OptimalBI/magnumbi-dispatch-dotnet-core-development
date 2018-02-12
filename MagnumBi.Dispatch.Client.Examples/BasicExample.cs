using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

// ReSharper disable ArgumentsStyleNamedExpression
// ReSharper disable ArgumentsStyleLiteral

namespace MagnumBi.Dispatch.Client.Examples {
    [SuppressMessage("ReSharper", "ArgumentsStyleStringLiteral")]
    public class BasicExample {
        private const string ExampleQueueId = "EXAMPLE_QUEUE";

        public static async Task<int> Main(string[] args) {
            int exitCode = 0;

            try {
                // Setup the MagnumBI Dispatch Client
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

                // Pop a new job on our example queue
                await client.QueueJob(ExampleQueueId, new {
                    Message = "Hello"
                });

                // Pop another job on the queue.
                await client.QueueJob(ExampleQueueId, new {
                    Message = "World"
                });

                // Now grab our jobs from the queue. 
                // This would typically be done by another program who is interested in the output from this application.
                DynamicJob job1 = await client.GetJob(
                    queueId: ExampleQueueId, // The queue that we want to get jobs from.
                    pollTimeout: -1, // How long we are happy with waiting for a job to appear. (If nothing appears it will come back with null.)
                    jobTimeout: 40 // Once we have the job, how long to wait until considering the job to have failed.
                );

                if (job1 == null) {
                    // if job is null then there was no job on the queue.
                    throw new Exception($"No job found on {ExampleQueueId}");
                }

                // Lets use the job data for something.
                Console.Write($"{job1.Data.Message}");

                // We need another job for this.
                Job<JobData> job2 = await client.GetJob<JobData>(
                    queueId: ExampleQueueId, // The queue that we want to get jobs from.
                    pollTimeout: -1, // How long we are happy with waiting for a job to appear.
                    jobTimeout: 40 // Once we have the job, how long to wait until considering the job to have failed.
                );

                if (job2 == null) {
                    // if job is null then there was no job on the queue.
                    throw new Exception($"No job found on {ExampleQueueId}");
                }

                // Use the job2's data.
                Console.Write($" {job2.Data.Message}\n");

                // Now that we have used the jobs successfully lets tell the dispatch server that we have done those jobs successfully.
                await job1.Complete();
                await job2.Complete();
                // Now these jobs wont accidentally get assigned to another program.

                // Lets also remove all other jobs from the queue, for fun.
                await client.ClearQueue(ExampleQueueId);

                // We are done.
                Console.WriteLine($"Examples completed successfully.");
            } catch (Exception e) {
                Console.Error.WriteLine($"\nCaught unexpected exception {e.Message}");
#if DEBUG
                Console.Error.WriteLine(e.StackTrace);
                throw;
#endif
            }
            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
            return exitCode;
        }

        public class JobData {
            public string Message { get; set; }
        }
    }
}