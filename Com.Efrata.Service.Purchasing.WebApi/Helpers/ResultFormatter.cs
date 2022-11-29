using Com.Moonlay.NetCore.Lib.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.WebApi.Helpers
{
    public class ResultFormatter
    {
        public Dictionary<string, object> Result { get; set; }

        public ResultFormatter(string ApiVersion, int StatusCode, string Message)
        {
            Result = new Dictionary<string, object>();
            AddResponseInformation(Result, ApiVersion, StatusCode, Message);
        }

        public Dictionary<string, object> Ok()
        {
            return Result;
        }

        public Dictionary<string, object> Ok(object Data)
        {
            Result.Add("data", Data);
            return Result;
        }

        public Dictionary<string, object> Ok(object Data, object Info)
        {
            Result.Add("data", Data);
            Result.Add("info", Info);
            return Result;
        }

        public Dictionary<string, object> Fail(string Error)
        {
            Result.Add("error", Error);
            return Result;
        }

        public Dictionary<string, object> Fail()
        {
            Result.Add("error", "Request Failed");
            return Result;
        }

        public Dictionary<string, object> Fail(ServiceValidationExeption e)
        {
            Dictionary<string, object> Errors = new Dictionary<string, object>();

            foreach (ValidationResult error in e.ValidationResults)
            {
                string key = error.MemberNames.First();

                try
                {
                    Errors.Add(error.MemberNames.First(), JsonConvert.DeserializeObject(error.ErrorMessage));
                }
                catch (Exception)
                {
                    Errors.Add(error.MemberNames.First(), error.ErrorMessage);
                }
            }

            Result.Add("error", Errors);
            return Result;
        }

        public void AddResponseInformation(Dictionary<string, object> Result, string ApiVersion, int StatusCode, string Message)
        {
            Result.Add("apiVersion", ApiVersion);
            Result.Add("statusCode", StatusCode);
            Result.Add("message", Message);
        }
    }
}
