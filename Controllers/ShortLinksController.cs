using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Linkster.Models.Links;
using Linkster.ViewModels.Links;
using Microsoft.AspNetCore.Mvc;

namespace Linkster.Controllers
{
    [Route("short-links")]
    public class ShortLinksController : Controller
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly DynamoDBContext _dbContext;

        public ShortLinksController(IAmazonDynamoDB dynamoDbClient)
        {
            AWSConfigsDynamoDB.Context.TypeMappings[typeof(ShortLink)] = new Amazon.Util.TypeMapping(typeof(ShortLink), ShortLink.TableName);

            this._dynamoDbClient = dynamoDbClient;
            this._dbContext = new DynamoDBContext(this._dynamoDbClient, new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 });
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            if (!await DoesTableExist())
            {
                await CreateTable();
            }

            var model = new ShortLinkListViewModel();

            var batchLinkGet = _dbContext.CreateBatchGet<ShortLink>();
            var search = _dbContext.ScanAsync<ShortLink>(new List<ScanCondition>());
            foreach(var task in await search.GetRemainingAsync())
            {
                batchLinkGet.AddKey(task.Key);
            }

            await batchLinkGet.ExecuteAsync();
            model.Links = batchLinkGet.Results;

            return View(model);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([Bind(Prefix = "NewLink")] NewLinkViewModel model)
        {
            // try
            // {
                var link = new ShortLink
                {
                    Key = Guid.NewGuid().ToString().Substring(0, 7),
                    DestinationUrl = model.DestinationUrl,
                    Created = DateTime.UtcNow
                };

                await _dbContext.SaveAsync<ShortLink>(link);

                return RedirectToAction(nameof(Index));
            // }
            // catch(exce)
        }

        private async Task<bool> DoesTableExist() 
        {
            var dynamoTables = await _dynamoDbClient.ListTablesAsync();
            var lastEvalTableName = string.Empty;

            var tableExists = dynamoTables.TableNames.Contains(ShortLink.TableName);
            if (tableExists)
            {
                return true;
            }

            while(!tableExists && !string.IsNullOrEmpty(lastEvalTableName))
            {
                dynamoTables = await _dynamoDbClient.ListTablesAsync(lastEvalTableName);
                tableExists = dynamoTables.TableNames.Contains(ShortLink.TableName);

                if (!tableExists)
                {
                    lastEvalTableName = dynamoTables.LastEvaluatedTableName;
                }
            }
            
            return false;
        }

        private async Task CreateTable()
        {
            var createTableRequest = new CreateTableRequest
            {
                TableName = ShortLink.TableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = nameof(ShortLink.Key),
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement 
                    {
                        AttributeName = nameof(ShortLink.Key),
                        KeyType = "HASH"
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput(5, 5)
            };

            await _dynamoDbClient.CreateTableAsync(createTableRequest);
        }
    }
}