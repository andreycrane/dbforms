/*
 * Created by SharpDevelop.
 * User: admin
 * Date: 01.09.2014
 * Time: 21:08
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
	/// Description of CountryCitys.
	/// </summary>
	public class CountryCitys : BrowseForm
	{
		private MySqlConnection conn;
		private DataRow countryRow;
		private DataTable tbl;
		
		public CountryCitys(MySqlConnection conn, DataRow row)
		{
			this.conn = conn;
			this.countryRow = row;
			this.tbl = new DataTable("city");
			this.DataSource = this.tbl;
			
			this.Title = "Города страны \'" + row.Field<string>("Name") + "\'";
		
			this.ButtonCreate = false;
			this.ButtonEdit = false;
			this.ButtonDelete = false;
			
			this.AddColumn("Name", "Название");
			this.AddColumn("District", "Столица");
			this.AddColumn("Population", "Численность населения");
		}
		
		protected sealed override void formLoad(object sender, EventArgs e)
		{
			string sql = @"select 
								id, 
								name, 
								countrycode, 
								district, 
								population 
							from city
							where countrycode = @countrycode";
			MySqlCommand cmd = new MySqlCommand(sql, this.conn);
			cmd.Parameters.Add(new MySqlParameter("@CountryCode", MySqlDbType.VarChar, 3)
			                   { Value = this.countryRow.Field<string>("Code") });
			
			MySqlDataReader reader = cmd.ExecuteReader();
			this.tbl.Load(reader);
		}
	}
}
