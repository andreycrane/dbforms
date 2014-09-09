/*
 * Created by SharpDevelop.
 * User: admin
 * Date: 01.09.2014
 * Time: 20:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;
using MySql.Data.MySqlClient;
using DbForms;

namespace TestForms
{
	/// <summary>
	/// Description of CountryLanguages.
	/// </summary>
	public class CountryLanguages : BrowseForm
	{
		private MySqlConnection conn;
		private DataRow countryRow;
		private DataTable tbl;
		
		public CountryLanguages(MySqlConnection conn, DataRow row)
		{	
			this.conn= conn;
			this.countryRow = row;
			this.Title = "Языки страны \'" + row.Field<string>("Name") + "\'";
			
			this.ButtonCreate = false;
			this.ButtonEdit = false;
			this.ButtonDelete = false;
			
			this.tbl = new DataTable("country");
		    this.DataSource = this.tbl;
			
		    this.AddColumn("Language", "Язык", 200);
		    this.AddColumn("IsOfficial", "Официальный");
		    this.AddColumn("Percentage", "Процент");
		}
		
		protected sealed override void formLoad(object sender, EventArgs e)
		{
			string sql = @"select
								CountryCode,
								Language,
								IsOfficial,
								Percentage
							from countrylanguage
							where CountryCode = @CountryCode";
			
			MySqlCommand cmd = new MySqlCommand(sql, this.conn);
			cmd.Parameters.Add(new MySqlParameter("@CountryCode", MySqlDbType.VarChar, 3)
			                   { Value = this.countryRow.Field<string>("Code") });
			MySqlDataReader reader = cmd.ExecuteReader();
			this.tbl.Load(reader);
		}
	}
}
