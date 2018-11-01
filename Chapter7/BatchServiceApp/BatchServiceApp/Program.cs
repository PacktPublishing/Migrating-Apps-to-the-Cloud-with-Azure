using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Auth;
using Microsoft.Azure.Batch.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchServiceApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Creating Batch service to process data on 3 codes in parallel
            for (int i = 0; i < 3; i++)
            {
                var processTask = new Task(() => RunProcess("Code" + i),TaskCreationOptions.LongRunning);
                processTask.Start();
            }

            Console.ReadLine();
        }

        static void RunProcess(string code)
        {
            BatchSharedKeyCredentials cred = new BatchSharedKeyCredentials("https://<Account Name>.<Region>.batch.azure.com", "<AccountName", "<Account Key>");
            BatchClient client = BatchClient.Open(cred);

            AddApplicationPool(client, code);
            AddCloudJob(client, code);

            // Remove if any tasks already exists
            DeleteCloudTask(client, code);
            AddCloudTask(client, code);

            DeleteCloudJob(client, code);
            DeleteApplicationPool(client, code);
        }

        static void AddApplicationPool(BatchClient client, string code)
        {
            var poolId = "applicationpool" + code;
            IPagedEnumerable<CloudPool> pools = client.PoolOperations.ListPools();
            foreach (CloudPool pool in pools)
            {
                if (pool.Id.Equals(poolId))
                {
                    Console.WriteLine("Pool already available for id : " + pool.Id);
                    return;
                }
            }
            
            CloudPool newPool = client.PoolOperations.CreatePool(
                    poolId: poolId,
                    targetDedicatedComputeNodes: 3,                                             // 3 compute nodes
                    virtualMachineSize: "small",                                                // single-core, 1.75 GB memory, 225 GB disk
                    cloudServiceConfiguration: new CloudServiceConfiguration(osFamily: "3"));

            newPool.Commit();
            Console.WriteLine("Created the pool for Code : " + code);
        }

        static CloudJob AddCloudJob(BatchClient client, string code)
        {
            var poolId = "applicationpool" + code;
            var jobId = "cloudjob" + code;
            IPagedEnumerable<CloudJob> jobs = client.JobOperations.ListJobs();
            foreach (CloudJob job in jobs)
            {
                if (job.Id.Equals(jobId))
                {
                    Console.WriteLine("Job already available for id : " + job.Id);
                    return job;
                }
            }

            CloudJob newJob = client.JobOperations.CreateJob();
            newJob.Id = jobId;
            newJob.PoolInformation = new PoolInformation() { PoolId = poolId };
            newJob.Commit();
            Console.WriteLine("Created the Cloud job for Code : " + code);

            return newJob;
        }

        static void AddCloudTask(BatchClient client, string code)
        {
            var poolId = "applicationpool" + code;
            var jobId = "cloudjob" + code;

            CloudJob job = client.JobOperations.GetJob(jobId);

            ResourceFile programFile = new ResourceFile("http:////MyApp.exe","MyApp.exe");

            ResourceFile appConfigurationData = new ResourceFile("http:////MyApp.exe.config", "MyApp.exe.config");

            string taskName = "applicationtask" + code;

            CloudTask task = new CloudTask(taskName, "MyApp.exe " + code);
            List<ResourceFile> taskFiles = new List<ResourceFile>();
            taskFiles.Add(appConfigurationData);
            taskFiles.Add(programFile);
            task.ResourceFiles = taskFiles;
            job.AddTask(task);
            job.Commit();
            job.Refresh();


            client.Utilities.CreateTaskStateMonitor().WaitAll(job.ListTasks(),TaskState.Completed, new TimeSpan(0, 30, 0));
            Console.WriteLine("Process completed successfully for code :" + code);
            foreach (CloudTask taskInProgress in job.ListTasks())
            {
                Console.WriteLine("Process " + taskInProgress.Id + " Output:\n" + taskInProgress.GetNodeFile(Constants.StandardOutFileName).ReadAsString());
            }

        }

        static void DeleteCloudTask(BatchClient client, string code)
        {
            var jobId = "cloudjob" + code;
            IPagedEnumerable<CloudJob> jobs = client.JobOperations.ListJobs();
            foreach (CloudJob checkjob in jobs)
            {
                if (checkjob.Id.Equals(jobId))
                {
                    CloudJob job = client.JobOperations.GetJob(jobId);
                    foreach (CloudTask task in job.ListTasks())
                    {
                        task.Delete();
                    }
                }
            }

            Console.WriteLine("Cloud tasks deleted for code : " + code);
        }

        static void DeleteCloudJob(BatchClient client, string code)
        {
            var jobId = "cloudjob" + code;
            client.JobOperations.DeleteJob(jobId);
            Console.WriteLine("Cloud Job deleted for code : " + code);
        }

        static void DeleteApplicationPool(BatchClient client, string code)
        {
            var poolId = "applicationpool" + code;
            client.PoolOperations.DeletePool(poolId);
            Console.WriteLine("Application pool was deleted for code : " + code);
        }

    }
}
