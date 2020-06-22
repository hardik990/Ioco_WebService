using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ioco_WebService.Models
{
	public class Response
	{
		public Status status;
		public string message { get; set; }
		public dynamic data { get; set; }
	}

	public enum Status
	{

		OK = 200,
		InternalServerError = 500,
		InvalidParameter = 503,
		BadRequest = 400
	}
}