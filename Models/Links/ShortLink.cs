using System;
using Amazon.DynamoDBv2.DataModel;

namespace Linkster.Models.Links 
{
    [DynamoDBTable(ShortLink.TableName)]
    public class ShortLink
    {
        public const string TableName = "ShortLinks";

        [DynamoDBHashKey]
        public string Key { get; set; }

        [DynamoDBProperty]
        public string DestinationUrl { get; set; }

        [DynamoDBProperty]
        public DateTime Created { get; set;}

    }
}