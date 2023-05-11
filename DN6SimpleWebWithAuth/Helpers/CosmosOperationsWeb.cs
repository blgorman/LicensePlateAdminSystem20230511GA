using Microsoft.ApplicationInsights;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LicensePlateProcessing.CosmosLogic
{
    public class CosmosOperationsWeb
    {
        private readonly string _endpointUrl; 
        private readonly string _authorizationKey; 
        private readonly string _databaseId; 
        private readonly string _containerId; 
        private static CosmosClient _client;
        private readonly TelemetryClient _telemetryClient;

        public CosmosOperationsWeb(string endpointURL, string authorizationKey, string databaseId, string containerId, TelemetryClient client)
        {
            _endpointUrl = endpointURL;
            _authorizationKey = authorizationKey;
            _databaseId = databaseId;
            _containerId = containerId;
            _telemetryClient = client;
        }

        /// <summary>
        /// Update the plates as confirmed but not exported
        /// </summary>
        /// <param name="fileName">name of the file</param>
        /// <param name="timeStamp">time of the file</param>
        public async Task UpdatePlatesForConfirmation(string fileName, DateTime timeStamp, string confirmedPlateText)
        {
            _telemetryClient.TrackTrace("Started processing for update plates for confirmation");

            int modifiedCount = 0;

            if (_client is null) _client = new CosmosClient(_endpointUrl, _authorizationKey);
            var container = _client.GetContainer(_databaseId, _containerId);

            //really this should just be one, but because of our repeated use of images, need to just mark them all
            //also, this query is likely expensive in cosmos RUs and could be optimized.
            using (FeedIterator<LicensePlateDataDocument> iterator = container.GetItemLinqQueryable<LicensePlateDataDocument>()
                      .Where(b => b.fileName == fileName)
                      .ToFeedIterator())
            {
                //Asynchronous query execution
                while (iterator.HasMoreResults)
                {
                    foreach (var item in await iterator.ReadNextAsync())
                    {
                        var match = timeStamp.Second == item.timeStamp.Second
                                    && timeStamp.Minute == item.timeStamp.Minute
                                    && timeStamp.Hour == item.timeStamp.Hour
                                    && timeStamp.Day == item.timeStamp.Day
                                    && timeStamp.Month == item.timeStamp.Month
                                    && timeStamp.Year == item.timeStamp.Year;
                        if (match)
                        {
                            _telemetryClient.TrackTrace($"Found {item.fileName} ready to update properties");
                            item.exported = false;
                            item.confirmed = true;
                            item.licensePlateText = confirmedPlateText;
                            var response = await container.ReplaceItemAsync(item, item.id);
                            _telemetryClient.TrackTrace($"Updated {item.fileName} as confirmed and ready for final export");
                            modifiedCount++;
                        }
                    }
                }
            }

            _telemetryClient.TrackTrace($"{modifiedCount} license plates found and marked as confirmed and ready for final export as per filename/timestamp");

        }
    }
}
