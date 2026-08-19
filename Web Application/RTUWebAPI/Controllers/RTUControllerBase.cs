using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RTUWebAPI.Models;

namespace RTUWebAPI.Controllers
{

    public class RTUControllerBase : ControllerBase
    {

        public ResponseMessage Results = new ResponseMessage();

        public void OnError(Exception e)
        {
            Results.ErrorMessage.Add(Guid.NewGuid().ToString(), new List<string> {e.Message});
        }

        public void OnError(string errMsg)
        {
            Results.ErrorMessage.Add(Guid.NewGuid().ToString(), new List<string> { errMsg } );
        }

        protected bool IsValidationErrors()
        {
            return (ModelState.Count(ms => ms.Value.Errors.Any()) != 0);
        }

        protected void PopulateResultsValidations()
        {
            if (IsValidationErrors())
            {
                var erroneousFields = ModelState.Where(ms => ms.Value.Errors.Any())
                            .Select(x => new { x.Key, x.Value.Errors });

                foreach (var erroneousField in erroneousFields)
                {
                    Results.ErrorMessage.Add(erroneousField.Key, erroneousField.Errors.Select(error => error.ErrorMessage ).ToList());

                }
            }
        }

        protected JsonResult JsonWithErrorMessages(object data)
        {
            PopulateResultsValidations();
            Results.Data = data;
            return new JsonResult(Results);
        }

    }
}