using HuaweiCloud.SDK.Core;
using HuaweiCloud.SDK.Core.Auth;
using HuaweiCloud.SDK.Ocr.V1;
using HuaweiCloud.SDK.Ocr.V1.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PassportOCR.Model;

namespace PassportOCR.Controllers
{
    public class OCRController : Controller
    {
        private IConfiguration _configuration;

        public OCRController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("api/passport-ocr")]
        public IActionResult PassportOCR([FromBody] PassportRequestBody reqModel)
        {
            var ak = _configuration.GetValue<string>("ak");
            var sk = _configuration.GetValue<string>("sk");
            var projectId = _configuration.GetValue<string>("projectId");
            var endPoint = "https://ocr.ap-southeast-1.myhuaweicloud.com"!;
            
            var config = HttpConfig.GetDefaultConfig();
            config.IgnoreSslVerification = true;
            var auth = new BasicCredentials(ak, sk, projectId);

            var client = OcrClient.NewBuilder()
                .WithCredential(auth)
                .WithRegion(OcrRegion.ValueOf("ap-southeast-1"))
                .WithHttpConfig(config)
                .Build();

            var request = new RecognizePassportRequest { };
            request.Body = reqModel;

            try
            {
                var response = client.RecognizePassport(request);
                var responseStatusCode = response.HttpStatusCode;
                if (responseStatusCode == 200)
                {
                    return Ok(JsonConvert.SerializeObject(response));
                }
            }
            catch (ServiceResponseException clientRequestException)
            {
                var errorDetails = $"Error Code: {clientRequestException.ErrorCode}, " +
                           $"Message: {clientRequestException.ErrorMessage}, " +
                           $"Request ID: {clientRequestException.RequestId}";
                return BadRequest(errorDetails);
            }

            return View();
        }
    }
}
