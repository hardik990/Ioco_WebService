using Ioco_WebService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Ioco_WebService.DBContext
{
	public class IocoDBcontext
	{

		public KeyValuePair<bool, string> AddInvoice(Invoice _Invoice)
		{
			if (checkTable())
			{
				using (var conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString))
				{
					if (conn.State == ConnectionState.Closed)
					{
						conn.Open();
					}
					string sqlcommand = "INSERT INTO invoice(client,vatRate,invoiceDate) VALUES (@client,@VatRate,@invoideDate) ; SELECT SCOPE_IDENTITY()";
					using (var sqlcmd = new SqlCommand(sqlcommand, conn))
					{
						sqlcmd.CommandType = CommandType.Text;

						sqlcmd.Parameters.AddWithValue("@client", _Invoice.Client);
						sqlcmd.Parameters.AddWithValue("@VatRate", _Invoice.VatRate);
						sqlcmd.Parameters.AddWithValue("@invoideDate", DateTime.Now);
						var ID = sqlcmd.ExecuteScalar();

						sqlcommand = "INSERT INTO invoiceLineItem(InvoiceId,quantity,description,unitPrice) VALUES (@InvoiceId,@quantity,@description,@unitPrice)";
						sqlcmd.CommandText = sqlcommand;
						sqlcmd.CommandType = CommandType.Text;
						foreach (var e1 in _Invoice.LineItem)
						{

							sqlcmd.Parameters.Clear();
							sqlcmd.Parameters.AddWithValue("@InvoiceId", ID);
							sqlcmd.Parameters.AddWithValue("@quantity", e1.quantity);
							sqlcmd.Parameters.AddWithValue("@description", e1.description);
							sqlcmd.Parameters.AddWithValue("@unitPrice", e1.unitPrice);
							sqlcmd.ExecuteNonQuery();
						}
					}
					if (conn.State == ConnectionState.Open)
					{
						conn.Close();
					}
				}
				return new KeyValuePair<bool, string>(true, "Record Inserted Successfully.");
			}
			else
			{
				return new KeyValuePair<bool, string>(false, "Database Not Found.");
			}
		}
		public KeyValuePair<bool, dynamic> GetInvoice()
		{
			if (checkTable())
			{
				List<Invoice> lstresult = new List<Invoice>();
				using (var conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString))
				{
					if (conn.State == ConnectionState.Closed)
					{
						conn.Open();
					}
					string sqlcommand = "Select * from  invoice";
					using (var sqlcmd = new SqlCommand(sqlcommand, conn))
					{
						sqlcmd.CommandType = CommandType.Text;
						using (SqlDataReader Dr = sqlcmd.ExecuteReader())
						{
							while (Dr.Read())
							{
								Invoice objdata = new Invoice();
								objdata.Id = long.Parse(Dr["Id"].ToString());
								objdata.Client = Dr["client"].ToString();
								objdata.VatRate = long.Parse(Dr["VatRate"].ToString());
								objdata.InvoiceDate = DateTime.Parse(Dr["InvoiceDate"].ToString());

								lstresult.Add(objdata);
							}
						}

						sqlcommand = "SELECT  * FROM invoiceLineItem WHERE InvoiceId = @InvoiceId";
						sqlcmd.CommandText = sqlcommand;
						sqlcmd.CommandType = CommandType.Text;
						foreach (var e1 in lstresult)
						{
							sqlcmd.Parameters.Clear();
							e1.LineItem = new List<LineItem>();
							sqlcmd.Parameters.AddWithValue("@InvoiceId", e1.Id);
							using (SqlDataReader Dr = sqlcmd.ExecuteReader())
							{
								while (Dr.Read())
								{
									LineItem objlstItem = new LineItem();
									objlstItem.Id = long.Parse(Dr["Id"].ToString());
									objlstItem.InvoiceId = long.Parse(Dr["InvoiceId"].ToString());
									objlstItem.quantity = long.Parse(Dr["quantity"].ToString());
									objlstItem.description = Dr["Id"].ToString();
									objlstItem.unitPrice = decimal.Parse(Dr["unitPrice"].ToString());
									objlstItem.LineItemtotal = objlstItem.getLineItem();
									e1.LineItem.Add(objlstItem);

									e1.SubTotal = e1.getSubTotal().ToString();
									e1.Vat = e1.getVat().ToString();
									e1.Total = e1.getTotal().ToString();
								}
							}
						}
					}
					if (conn.State == ConnectionState.Open)
					{
						conn.Close();
					}
				}
				return new KeyValuePair<bool, dynamic>(true, lstresult);
			}
			else
			{
				return new KeyValuePair<bool, dynamic>(false, "Database Not Found.");
			}
		}
		public KeyValuePair<bool, dynamic> GetInvoiceById(long invoiceID)
		{
			if (checkTable())
			{
				Invoice lstresult = new Invoice();
				using (var conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString))
				{
					if (conn.State == ConnectionState.Closed)
					{
						conn.Open();
					}
					string sqlcommand = "Select Top 1 * from  invoice where Id =@invoiceID";
					using (var sqlcmd = new SqlCommand(sqlcommand, conn))
					{
						sqlcmd.CommandType = CommandType.Text;
						sqlcmd.Parameters.AddWithValue("@invoiceID", invoiceID);
						using (SqlDataReader Dr = sqlcmd.ExecuteReader())
						{
							while (Dr.Read())
							{
								lstresult.Id = long.Parse(Dr["Id"].ToString());
								lstresult.Client = Dr["client"].ToString();
								lstresult.VatRate = long.Parse(Dr["VatRate"].ToString());
								lstresult.InvoiceDate = DateTime.Parse(Dr["InvoiceDate"].ToString());
							}
						}

						sqlcommand = "SELECT  * FROM invoiceLineItem WHERE InvoiceId = @InvoiceId";
						sqlcmd.CommandText = sqlcommand;
						sqlcmd.CommandType = CommandType.Text;
						sqlcmd.Parameters.Clear();
						lstresult.LineItem = new List<LineItem>();
						sqlcmd.Parameters.AddWithValue("@InvoiceId", lstresult.Id);
						using (SqlDataReader Dr = sqlcmd.ExecuteReader())
						{
							while (Dr.Read())
							{
								LineItem objlstItem = new LineItem();
								objlstItem.Id = long.Parse(Dr["Id"].ToString());
								objlstItem.InvoiceId = long.Parse(Dr["InvoiceId"].ToString());
								objlstItem.quantity = long.Parse(Dr["quantity"].ToString());
								objlstItem.description = Dr["Id"].ToString();
								objlstItem.unitPrice = decimal.Parse(Dr["unitPrice"].ToString());
								lstresult.LineItem.Add(objlstItem);
							}
						}
					}
					if (conn.State == ConnectionState.Open)
					{
						conn.Close();
					}
				}
				return new KeyValuePair<bool, dynamic>(true, lstresult);
			}
			else
			{
				return new KeyValuePair<bool, dynamic>(false, "Database Not Found.");
			}
		}
		private bool checkTable()
		{
			string checkDB = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\ioco.mdf";
			if (!File.Exists(checkDB))
			{
				//Create .mdf file 
				string ldfDB = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\ioco.ldf";
				SqlConnection myConn = new SqlConnection(@"Server=(LocalDB)\MSSQLLocalDB;Integrated security=SSPI;database=master");

				string str = "CREATE DATABASE MyDatabase ON PRIMARY " +
					"(NAME = MyDatabase_Data, " +
					"FILENAME = '" + checkDB + "', " +
					 "SIZE = 2MB, MAXSIZE = 10MB, FILEGROWTH = 10%) " +
					 "LOG ON (NAME = ioco_Log, " +
					 "FILENAME = '" + ldfDB + "', " +
					 "SIZE = 50MB, " +
					 "MAXSIZE = 1GB, " +
					 "FILEGROWTH = 10%)";
				SqlCommand myCommand = new SqlCommand(str, myConn);
				try
				{
					myConn.Open();
					myCommand.ExecuteNonQuery();
					//MessageBox.Show("DataBase is Created Successfully", "MyProgram", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				catch (System.Exception ex)
				{
					//MessageBox.Show(ex.ToString(), "MyProgram", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				finally
				{
					if (myConn.State == ConnectionState.Open)
					{
						myConn.Close();
					}
				}
			}
			using (var conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString))
			{
				if (conn.State == ConnectionState.Closed)
				{
					conn.Open();
				}
				string sqltest = "select case when exists((select * from information_schema.tables where table_name = '" + "invoice" + "')) then 1 else 0 end";
				using (var sqlcmd = new SqlCommand(sqltest, conn))
				{
					sqlcmd.CommandType = CommandType.Text;
					if ((int)sqlcmd.ExecuteScalar() == 1)
					{
						return true;
					}
					else
					{
						createtable();
					}
				}
				if (conn.State == ConnectionState.Open)
				{
					conn.Close();
				}
			}
			return true;
		}
		private void createtable()
		{
			using (var conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString))
			{
				if (conn.State == ConnectionState.Closed)
				{
					conn.Open();
				}
				string sqltest = string.Empty;
				string path = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\DB.txt";
				string[] lines = System.IO.File.ReadAllLines(path);
				foreach (string line in lines)
				{
					sqltest = sqltest + line + new string(' ', 10);
				}
				using (var sqlcmd = new SqlCommand(sqltest, conn))
				{
					sqlcmd.CommandType = CommandType.Text;
					sqlcmd.ExecuteScalar();



				}
			}
		}
	}
}