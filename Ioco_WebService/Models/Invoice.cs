using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace Ioco_WebService.Models
{
	public class Invoice
	{
		public long Id { get; set; }

		public string Client { get; set; }

		public long VatRate { get; set; }
		public DateTime InvoiceDate { get; set; }

		public string SubTotal { get; set; }
		public decimal getSubTotal()
		{
			return LineItem.Sum(x => x.getLineItem());
		}

		public string Vat { get; set; }
		public decimal getVat()
		{
			return (getSubTotal() * VatRate) / 100;
		}

		public string Total { get; set; }
		public decimal getTotal()
		{
			return getSubTotal() + getVat();
		}

		public List<LineItem> LineItem { get; set; }
	}

	public class LineItem
	{
		public long Id { get; set; }
		public long InvoiceId { get; set; }
		public long quantity { get; set; }
		public string description { get; set; }
		public decimal unitPrice { get; set; }

		public decimal LineItemtotal { get; set; }
		public decimal getLineItem()
		{
			return quantity * unitPrice;
		}
	}
}