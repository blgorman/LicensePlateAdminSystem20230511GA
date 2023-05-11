using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LicensePlateDataModels;
using LicensePlateDataLibrary;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Azure.ServiceBus;
using System.Diagnostics;
using Microsoft.ApplicationInsights;
using LicensePlateProcessing.CosmosLogic;

namespace LicensePlateAdminSystem.Controllers
{
    public class LicensePlatesController : Controller
    {
        private readonly LicensePlateDataDbContext _context;
        private readonly string _SASToken;
        private static string _queueConnectionString;
        private static string _queueName;
        private static string _cosmosEndpoint;
        private static string _cosmosAuthKey;
        private static string _cosmosDbId;
        private static string _cosmosContainer;
        private readonly TelemetryClient _telemetryClient;

        public LicensePlatesController(LicensePlateDataDbContext context, TelemetryClient client)
        {
            _context = context;
            _SASToken = Environment.GetEnvironmentVariable("PlateImagesSASToken");
            _queueConnectionString = Environment.GetEnvironmentVariable("ReadOnlySBConnectionString");
            _queueName = Environment.GetEnvironmentVariable("ServiceBusQueueName");
            _cosmosEndpoint = Environment.GetEnvironmentVariable("cosmosDBEndpointUrl");
            _cosmosAuthKey = Environment.GetEnvironmentVariable("cosmosDBAuthorizationKey");
            _cosmosContainer = Environment.GetEnvironmentVariable("cosmosDBContainerId");
            _cosmosDbId = Environment.GetEnvironmentVariable("cosmosDBDatabaseId");
            _telemetryClient = client;
        }

        // GET: LicensePlates
        public async Task<IActionResult> Index(string? success)
        {
            if (!string.IsNullOrWhiteSpace(success) && success.Equals("showsuccess"))
            {
                ViewBag.JavaScriptFunction = "notifyUserOfSuccess";
            }

            return _context.LicensePlates != null ?
                        View(await _context.LicensePlates.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.LicensePlates'  is null.");
        }

        // GET: LicensePlates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.LicensePlates == null || !_context.LicensePlates.Any())
            {
                return NotFound();
            }

            var licensePlateData = await _context.LicensePlates
                .FirstOrDefaultAsync(m => m.Id == id);

            if (licensePlateData == null)
            {
                return NotFound();
            }

            ViewBag.ImageURL = $"{licensePlateData.FileName}?{_SASToken}";
            return View(licensePlateData);
        }

        // GET: LicensePlates/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LicensePlates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IsProcessed,FileName,LicensePlateText,TimeStamp")] LicensePlate licensePlateData)
        {
            if (ModelState.IsValid)
            {
                _context.Add(licensePlateData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(licensePlateData);
        }

        // GET: LicensePlates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.LicensePlates == null)
            {
                return NotFound();
            }

            var licensePlateData = await _context.LicensePlates.FindAsync(id);
            if (licensePlateData == null)
            {
                return NotFound();
            }
            return View(licensePlateData);
        }

        // POST: LicensePlates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IsProcessed,FileName,LicensePlateText,TimeStamp")] LicensePlate licensePlateData)
        {
            if (id != licensePlateData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(licensePlateData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LicensePlateDataExists(licensePlateData.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(licensePlateData);
        }

        // GET: LicensePlates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.LicensePlates == null)
            {
                return NotFound();
            }

            var licensePlateData = await _context.LicensePlates
                .FirstOrDefaultAsync(m => m.Id == id);
            if (licensePlateData == null)
            {
                return NotFound();
            }

            return View(licensePlateData);
        }

        // POST: LicensePlates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.LicensePlates == null)
            {
                return Problem("Entity set 'ApplicationDbContext.LicensePlates'  is null.");
            }
            var licensePlateData = await _context.LicensePlates.FindAsync(id);
            if (licensePlateData != null)
            {
                _context.LicensePlates.Remove(licensePlateData);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LicensePlateDataExists(int id)
        {
            return (_context.LicensePlates?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        // GET: LicensePlates/ReviewNextPlateFromQueue
        public async Task<IActionResult> ReviewNextPlateFromQueue()
        {
            var messageBody = string.Empty;
            var lpd = new LicensePlateQueueMessageData();
            try
            {
                //https://www.michalbialecki.com/en/2018/05/21/receiving-only-one-message-from-azure-service-bus/
                var queueClient = new QueueClient(_queueConnectionString, _queueName);

                queueClient.RegisterMessageHandler(
                async (message, token) =>
                {
                    messageBody = Encoding.UTF8.GetString(message.Body);
                    _telemetryClient.TrackEvent($"Received: {messageBody}");

                    lpd = JsonConvert.DeserializeObject<LicensePlateQueueMessageData>(messageBody);
                    _telemetryClient.TrackTrace($"LPD converted: {lpd.LicensePlateText} | {lpd.FileName}");

                    await queueClient.CompleteAsync(message.SystemProperties.LockToken);
                    await queueClient.CloseAsync();
                },
                new MessageHandlerOptions(async args => _telemetryClient.TrackException(args.Exception))
                { MaxConcurrentCalls = 1, AutoComplete = true });

                _telemetryClient.TrackTrace($"Message: {messageBody}");
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
            }

            var autoTimeOut = DateTime.Now.AddSeconds(30);
            while (string.IsNullOrWhiteSpace(lpd?.FileName) && DateTime.Now < autoTimeOut)
            { 
                Thread.Sleep(1000);
            }

            if (string.IsNullOrWhiteSpace(lpd?.FileName))
            {
                _telemetryClient.TrackException(new Exception("No data returned from the queue for processing"));
                return RedirectToAction(nameof(Index));
            }
            //inject the sas token on the url
            var imageURL = $"{lpd?.FileName}?{_SASToken}";
            _telemetryClient.TrackTrace($"ImageURL: {imageURL}");

            ViewBag.ImageURL = imageURL;

            //open the review page with the LicensePlateData
            return View(lpd);
        }

        // POST: LicensePlates/UpdateCosmos/lpd object
        [HttpPost, ActionName("UpdateCosmos")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCosmos([Bind("LicensePlateText, FileName, TimeStamp")] LicensePlateQueueMessageData licensePlateData)
        {
            var plateData = new Dictionary<string, string>();
            plateData.Add(licensePlateData.FileName, licensePlateData.LicensePlateText);
            _telemetryClient.TrackEvent("User updating plate", plateData);

            var cosmosHelper = new CosmosOperationsWeb(_cosmosEndpoint, _cosmosAuthKey, _cosmosDbId, _cosmosContainer, _telemetryClient);
            await cosmosHelper.UpdatePlatesForConfirmation(licensePlateData.FileName, licensePlateData.TimeStamp, licensePlateData.LicensePlateText);
            _telemetryClient.TrackTrace($"Completed processing for file {licensePlateData.FileName} with ts {licensePlateData.TimeStamp}");

            return RedirectToAction(nameof(Index), new { success = "showsuccess" });
        }
    }
}
