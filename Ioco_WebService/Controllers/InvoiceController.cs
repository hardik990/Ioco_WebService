using Ioco_WebService.DBContext;
using Ioco_WebService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Ioco_WebService.Controllers
{
    public class InvoiceController : ApiController
    {
        IocoDBcontext _IocoDBcontext { get; set; }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("invoice")]
        public HttpResponseMessage addInvoice([FromBody]Invoice invoice)
        {
            var result = new Response();
            try
            {
                if (_IocoDBcontext == null)
                    _IocoDBcontext = new IocoDBcontext();
                KeyValuePair<bool, string> isValid = _IocoDBcontext.AddInvoice(invoice);
                if (isValid.Key)
                {
                    result.data = isValid.Value;
                    result.status = Status.OK;
                    result.message = string.Empty;
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                else
                {
                    result.status = Status.InternalServerError;
                    result.message = isValid.Value.ToString();
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, result);
                }
            }
            catch (Exception Ex)
            {
                result.status = Status.BadRequest;
                result.message = Ex.ToString();
                return Request.CreateResponse(HttpStatusCode.BadRequest, result); 
            }   
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("invoice")]
        public HttpResponseMessage viewAllInvoice()
        {
            var result = new Response();
            try
            {
                if (_IocoDBcontext == null)
                    _IocoDBcontext = new IocoDBcontext();
                KeyValuePair<bool, dynamic> isValid = _IocoDBcontext.GetInvoice();
                if (isValid.Key)
                {
                    result.data = isValid.Value;
                    result.status = Status.OK;
                    result.message = string.Empty;
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                else
                {
                    result.status = Status.InternalServerError;
                    result.message = isValid.Value.ToString();
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, result);
                }
            }
            catch (Exception Ex)
            {
                result.status = Status.BadRequest;
                result.message = Ex.ToString();
                return Request.CreateResponse(HttpStatusCode.BadRequest, result);
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("invoice/{invoiceID}")]
        public HttpResponseMessage viewInvoice(long invoiceID)
        {
            var result = new Response();
            try
            {
                if (_IocoDBcontext == null)
                    _IocoDBcontext = new IocoDBcontext();
                KeyValuePair<bool, dynamic> isValid = _IocoDBcontext.GetInvoiceById(invoiceID);
                if (isValid.Key)
                {
                    result.data = isValid.Value;
                    result.status = Status.OK;
                    result.message = string.Empty;
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                else
                {
                    result.status = Status.InternalServerError;
                    result.message = isValid.Value.ToString();
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, result);
                }
            }
            catch (Exception Ex)
            {
                result.status = Status.BadRequest;
                result.message = Ex.ToString();
                return Request.CreateResponse(HttpStatusCode.BadRequest, result);
            }
        }
    }
}