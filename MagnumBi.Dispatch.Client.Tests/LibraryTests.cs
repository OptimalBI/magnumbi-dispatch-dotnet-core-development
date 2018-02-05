using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace MagnumBi.Dispatch.Client.Tests{
    public class LibraryTests : IDisposable{
        private readonly MagnumBiDispatchClient client;

        public LibraryTests(){
            this.client = new MagnumBiDispatchClient("https://127.0.0.1:6883", "test", "token", false);
            this.CleanUp();
        }

        [Fact]
        public void AlwaysPassingTest(){
            Assert.True(true);
        }

        [Fact]
        public async void TestStatusCode(){
            bool res = await this.client.CheckStatus();
            Assert.True(res);
        }

        [Fact]
        public async void TestGetStatusCode(){
            HttpStatusCode code = await this.client.CheckStatusCode();
            Assert.Equal(HttpStatusCode.OK, code);
        }

        [Fact]
        public async void TestGetJobOne(){
            Job job = await this.client.GetJob("TEST", -1, 5);
            Assert.Null(job); // Should be no jobs as queue should be empty.
        }

        [Fact]
        public void TestQueueJobOne(){
            Task queueTask = this.client.QueueJob("TEST", new{Thing = "thing", obj = new{test = "more"}});
            Task.WaitAll(queueTask);
        }

        [Fact]
        public async void TestJobProcessOne(){
            Task queueTask = this.client.QueueJob("TEST", new{Thing = "thing", obj = new{test = "more"}});
            Task.WaitAll(queueTask);

            Job job = await this.client.GetJob("TEST", -1, 22);
            if (job != null) {
                Assert.False(string.IsNullOrWhiteSpace(job.JobId));
                Task completeTask = job.Complete();
                Task.WaitAll(completeTask);
            } else {
                throw new Exception("Job we just queued did not come back.");
            }
        }

        private void CleanUp(){
            Task<Job> task = this.client.GetJob("TEST", -1, 10);
            Task.WaitAll(task);
            Job job = task.Result;
            while (job != null) {
                Task t2 = job.Complete();
                Task.WaitAll(t2);
                task = this.client.GetJob("TEST", -1, 10);
                Task.WaitAll(task);
                job = task.Result;
            }
        }

        public void Dispose(){
            // Do stuff to clean up.
            this.CleanUp();
        }
    }
}