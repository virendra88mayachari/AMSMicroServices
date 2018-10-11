using System;
using System.IO;
using System.Threading.Tasks;
using AMSDemo.DatabaseOps;
using AMSDemo.Utility;
using Microsoft.AspNetCore.Mvc;

namespace AMSDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProcessDIALSController : ControllerBase
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpGet]
        //MicroService 3
        public async Task<IActionResult> ProcessDIALSDataNWriteToDB()
        {
            try
            {
                string dialsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "DIALSFiles");

                if (Directory.Exists(dialsFolderPath))
                {
                    var files = Directory.GetFiles(dialsFolderPath);

                    if (files.Length > 0)
                    {
                        foreach (string fileName in files)
                        {
                            log.Info(DateTime.Now.ToString() + " AMS-POC: DIALS file processing in progress - " + fileName);
                            FileStream fileStream = new FileStream(Path.Combine(dialsFolderPath, fileName), FileMode.Open);
                            using (BufferedStream bufferedStream = new BufferedStream(fileStream))
                            {
                                using (StreamReader streamReader = new StreamReader(bufferedStream))
                                {
                                    while (!streamReader.EndOfStream)
                                    {
                                        string dialsString = await streamReader.ReadLineAsync();

                                        //Process DIALS data
                                        var dialsObject = DIALSUtility.ProcessDIALSData(dialsString);

                                        //Store in to DB
                                        if (!string.IsNullOrEmpty(dialsObject.TrackingNumber))
                                        {
                                            SakilaContext context = HttpContext.RequestServices.GetService(typeof(SakilaContext)) as SakilaContext;
                                            context.AddNewDIALS(dialsObject);
                                        }
                                        else {
                                            log.Warn(DateTime.Now.ToString() + " AMS-POC: Tracking number not found in DIALS data.");
                                        }
                                    }
                                }
                            }
                            log.Info(DateTime.Now.ToString() + " AMS-POC: DIALS file processing completed - " + fileName);
                        }

                        log.Info(DateTime.Now.ToString() + " AMS-POC: Total DIALS files processed - " + files.Length);
                    }
                }
            }
            catch(Exception ex) {
                log.Error(DateTime.Now.ToString() + " AMS-POC: " + Convert.ToString(ex.Message));
                return new JsonResult(new { Result = System.Net.HttpStatusCode.InternalServerError });
            }

            return Ok();
        }

        public IActionResult TestMethod()
        {
            return Ok( );
        }
    }
}