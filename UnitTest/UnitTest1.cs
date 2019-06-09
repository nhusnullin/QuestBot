using CoreBot;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using ScenarioBot.Service;

namespace UnitTestProject
{
    public static class Settings
    {
        //public static string ConStr = "DefaultEndpointsProtocol=https;AccountName=0ecc2b45-0ee0-4-231-b9ee;AccountKey=Ej47ZXRBV7ZEGmNTIwl7w0HsoeCMmIZfGT5NNlz6iJWF7IZcJZJMnKg9v9PN7Wtf3X3Df2okTKooypJG3EjJUA==;TableEndpoint=https://0ecc2b45-0ee0-4-231-b9ee.table.cosmos.azure.com:443/;";
        public static string ConStr =
            "DefaultEndpointsProtocol=https;AccountName=dotquestbot;AccountKey=SpHHMi1pTPj2LCg03BuoKSnI4X0LRFf7eVyPaZN9JjugBJXS6YqYm6exbXIg4xyNtWkolOUDGyjv9g8SfGenOg==;TableEndpoint=https://dotquestbot.table.cosmos.azure.com:443/;";
    }

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void T2()
        {

//            var sc1 = JsonConvert.SerializeObject(InMemoryScenarioStore.GetScenario1(), Newtonsoft.Json.Formatting.None,
//                            new JsonSerializerSettings
//                            {
//                                NullValueHandling = NullValueHandling.Ignore
//                            });
        }


        [TestMethod]
        public async Task tt()
        {
            //var scenarioService = new ScenarioService();
            //scenarioService.LoadAll();


            //await InMemoryScenarioStore.Generate(scenarioService.Store["nukescenario"]);
        }

        [TestMethod]
//        [DataRow(User.TableName)]
//        [DataRow(Answer.TableName)]
        public async Task CreateUserTable(string tableName)
        {
            CloudTable table = await Common.CreateTableAsync(tableName);
        }

        [TestMethod]
        public async Task TestMethod1()
        {
            var tableName = "users";

            CloudTable table = await Common.CreateTableAsync(tableName);

            CustomerEntity customer = new CustomerEntity("Harp", "Walter")
            {
                Email = "Walter@contoso.com",
                PhoneNumber = "425-555-0101"
            };

            customer = await SamplesUtils.InsertOrMergeEntityAsync(table, customer);

            var customerII = new CustomerEntityII("asds", "qwe")
            {
                bla = "����"
            };

            customerII = await SamplesUtils.InsertOrMergeEntityAsync(table, customerII);
        }
    }

    public class CustomerEntityII : TableEntity
    {
        public CustomerEntityII()
        {
        }

        public CustomerEntityII(string lastName, string firstName)
        {
            PartitionKey = lastName;
            RowKey = firstName;
        }

        public string bla { get; set; }
    }

    public class CustomerEntity : TableEntity
    {
        public CustomerEntity()
        {
        }

        public CustomerEntity(string lastName, string firstName)
        {
            PartitionKey = lastName;
            RowKey = firstName;
        }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }

    class SamplesUtils
    {
        public static async Task<CustomerEntity> RetrieveEntityUsingPointQueryAsync(CloudTable table, string partitionKey, string rowKey)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<CustomerEntity>(partitionKey, rowKey);
                TableResult result = await table.ExecuteAsync(retrieveOperation);
                CustomerEntity customer = result.Result as CustomerEntity;
                if (customer != null)
                {
                    Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", customer.PartitionKey, customer.RowKey, customer.Email, customer.PhoneNumber);
                }

                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of Retrieve Operation: " + result.RequestCharge);
                }

                return customer;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public static async Task<T> InsertOrMergeEntityAsync<T>(CloudTable table, T entity) where T : ITableEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation.
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                //T insertedCustomer = result.Result as T;

                if (result.RequestCharge.HasValue)
                {
                    //Console.WriteLine("Request Charge of InsertOrMerge Operation: " + result.RequestCharge);
                }

                return (T)result.Result;
            }
            catch (StorageException e)
            {
                //Console.WriteLine(e.Message);
                //Console.ReadLine();
                throw;
            }
        }

        public static async Task DeleteEntityAsync(CloudTable table, CustomerEntity deleteEntity)
        {
            try
            {
                if (deleteEntity == null)
                {
                    throw new ArgumentNullException("deleteEntity");
                }

                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
                TableResult result = await table.ExecuteAsync(deleteOperation);

                if (result.RequestCharge.HasValue)
                {
                    //Console.WriteLine("Request Charge of Delete Operation: " + result.RequestCharge);
                }

            }
            catch (StorageException e)
            {
                throw;
            }
        }

    }

    public class Common
    {
        public static CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }

        public static async Task<CloudTable> CreateTableAsync(string tableName)
        {
            string storageConnectionString = Settings.ConStr;

            // Retrieve storage account information from connection string.
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(storageConnectionString);

            // Create a table client for interacting with the table service
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            Console.WriteLine("Create a Table for the demo");

            // Create a table client for interacting with the table service 
            CloudTable table = tableClient.GetTableReference(tableName);
            if (await table.CreateIfNotExistsAsync())
            {
                Console.WriteLine("Created Table named: {0}", tableName);
            }
            else
            {
                Console.WriteLine("Table {0} already exists", tableName);
            }

            Console.WriteLine();
            return table;
        }

        public static async Task<CloudTable> CreateTableAsync(CloudTableClient tableClient, string tableName)
        {
            Console.WriteLine("Create a Table for the demo");

            // Create a table client for interacting with the table service 
            CloudTable table = tableClient.GetTableReference(tableName);
            try
            {
                if (await table.CreateIfNotExistsAsync())
                {
                    Console.WriteLine("Created Table named: {0}", tableName);
                }
                else
                {
                    Console.WriteLine("Table {0} already exists", tableName);
                }
            }
            catch (StorageException)
            {
                Console.WriteLine(
                    "If you are running with the default configuration please make sure you have started the storage emulator. Press the�Windows�key and type Azure Storage to select and run it from the list of applications - then restart the sample.");
                Console.ReadLine();
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine();
            return table;
        }
    }
}