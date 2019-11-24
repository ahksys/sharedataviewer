using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShareDataController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        // IWebHostEnvironment is injected through the constructor to access the web 
        // content root path so we can reach the folder under which the CSV data file is stored.
        public ShareDataController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        }

        // Check if the minimum data requirements has been met. This is defined by the requirements.
        private DataCheckResult CheckDataFileMeetsMinimumRequirements( List<SharePrice> shareData )
        {
            try
            {
                var result = new DataCheckResult
                {
                    passed = true,
                    comments = ""
                };

                // Business rule 1: at least 3 historical data for 3 units
                int unitCount = shareData.Select(x => x.unitID).Distinct().Count();

                if( unitCount < 3)
                {
                    result.passed = false;
                    result.comments += "Data for 3 or more units required." + Environment.NewLine;
                }

                // Business rule 2: at least 7 days of historical data for each unit
                int numDaysPerUnit = shareData.GroupBy(x => x.unitID).Select(g => new
                {
                    unitId = g.Key,
                    count = g.Count()
                }).Min(x => x.count);

                if( numDaysPerUnit < 7)
                {
                    result.passed = false;
                    result.comments += "Minimum 7 days of data required for each unit." + Environment.NewLine;
                }

                result.comments = result.comments.Trim();

                return result;
            }
            catch ( Exception ex)
            {
                return new DataCheckResult
                {
                    passed = false,
                    comments = ex.Message
                };
            }
        }

        // GET: api/ShareData?displayOrder={displayOrderOption}
        // displayOrderOption includes:
        // "none" - display all data in descending date order
        // "leastExpensive" - display top 5 least expensive unit prices in ascending unit price order
        // "mostExpensive" - display top 5 most expensive unit prices in descending unit price order
        [HttpGet]
        public ActionResult<IEnumerable<SharePrice>> Get(string displayOrder)
        {
            try
            {
                List<SharePrice> outputData = null;

                // Create the full file path
                var dataFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "uploads", "SharePriceData.csv");

                // Check if the file exists, if not then return bad request
                if( !System.IO.File.Exists(dataFilePath))
                {
                    return BadRequest("No data found. Please upload a data file.");
                }

                // Open a stream to read the CSV file and let CsvReader process the file
                using (var reader = new StreamReader(dataFilePath))
                {
                    using (var csv = new CsvReader(reader))
                    {
                        // Retrieve the data based on the SharePrice data structure defined
                        var shareData = csv.GetRecords<SharePrice>().ToList();

                        // Check if the minimum data conditions are met
                        var checkResult = CheckDataFileMeetsMinimumRequirements(shareData);

                        if (!checkResult.passed)
                        {
                            // We did not pass the minimum data requirements. Display result to user and indicate the issue(s).
                            return BadRequest(checkResult.comments);
                        }

                        if (displayOrder == "leastExpensive")  // display top 5 least expensive unit prices in ascending unit price order
                        {
                            outputData = shareData.OrderBy(x => x.unitPrice).Take(5).ToList();
                        }
                        else if (displayOrder == "mostExpensive")  // display top 5 most expensive unit prices in descending unit price order
                        {
                            outputData = shareData.OrderByDescending(x => x.unitPrice).Take(5).ToList();
                        }
                        else
                        {
                            // By default, display the unit prices in data file in descending date order and by ascending unitID order
                            outputData = shareData.OrderBy(x => x.unitID).OrderByDescending(x => x.date).ToList();
                        }
                    }
                }

                return Ok(outputData);

            } catch( Exception ex )
            {
                Console.WriteLine("Exception: " + ex.ToString()) ;
                return BadRequest(ex.ToString());
            }

        }

        // POST: api/ShareData
        // Handle the saving of the data file into the designated "uploads" directory.
        [HttpPost]
        public async Task Post(IFormFile file)
        {
            try
            {
                var uploads = Path.Combine(_webHostEnvironment.ContentRootPath, "uploads");

                // create the "uploads" directory if it doesn't exist
                if( !Directory.Exists(uploads) )
                {
                    Directory.CreateDirectory(uploads);
                }

                // only proceed to save the file if the file is not empty
                if (file.Length > 0)
                {
                    using (var fs = new FileStream(Path.Combine(uploads, "SharePriceData.csv"), FileMode.Create))
                    {
                        await file.CopyToAsync(fs);
                    }
                }
            }
            catch ( Exception ex )
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }

    }
}
