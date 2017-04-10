using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Data;
using Mono.Data.Sqlite;
using System.DirectoryServices;
using System.Diagnostics;

namespace DemoSampleDMM.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			var mvcName = typeof(Controller).Assembly.GetName();
			var isMono = Type.GetType("Mono.Runtime") != null;

			ViewData["Version"] = mvcName.Version.Major + "." + mvcName.Version.Minor;
			ViewData["Runtime"] = isMono ? "Mono" : ".NET";

			return View();
		}

		public void GetUserInfo(string connection, string username)
		{


			const string connectionString = "URI=file:SqliteTest.db";
			var dbcon = new SqliteConnection(connectionString);
			dbcon.Open();
			var dbcmd = dbcon.CreateCommand();
			var sql = "SELECT * FROM UserAccount WHERE Username = '" + username + "'";
			dbcmd.CommandText = sql;
			var reader = dbcmd.ExecuteReader();
			while (reader.Read())
			{
				string firstName = reader.GetString(0);
				string lastName = reader.GetString(1);
				Console.WriteLine("Name: {0} {1}",
					firstName, lastName);
			}
			// clean up
			reader.Dispose();
			dbcmd.Dispose();
			dbcon.Close();
		}

		public string GetUserFullName(string loginName)
		{
			//Root entry for contoso.com global catalog
			DirectoryEntry root = new
			    DirectoryEntry("GC://DC=contoso,DC=com");

			//Instantiating directory searcher object
			DirectorySearcher searcher = new DirectorySearcher();
			root.AuthenticationType = AuthenticationTypes.Secure;
			searcher.SearchRoot = root;

			//Searching the directory for the specified windows login name
			searcher.Filter = "(sAMAccountName=" + loginName + ")";
			SearchResult result = searcher.FindOne();
			//Retrieving user common name
			string userName = result.Properties["cn"][0].ToString();
			root.Close();
			//Returning the user name
			return userName;
		}

		public void commandInjection()
		{
			string strCmdText = @"/C dir c:\files\" + Request.QueryString["dir"];
			ProcessStartInfo info = new ProcessStartInfo("CMD.exe", strCmdText);
			Process.Start(info);
		}
		public void RedirectVul()
		{
			Response.Redirect(Request.QueryString["Url"]);

			Server.Execute(Request.QueryString["file"]);
		}
	}
}
