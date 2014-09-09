/*
 * Created by SharpDevelop.
 * User: admin
 * Date: 25.08.2014
 * Time: 19:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using DbForms;

namespace TestForms
{
	/// <summary>
	/// Description of CityForm.
	/// </summary>
	public class CityBrowse : BrowseForm
	{
		private MySqlConnection conn;
		private DataTable tbl;
		
		public CityBrowse(MySqlConnection conn)
		{
			this.conn = conn;
			this.tbl = new DataTable("city");
			
			this.Title = "Города";
			
			this.AddColumn("Name", "Название", 200);
			this.AddColumn("CountryCode", "Код страны");
			this.AddColumn("District", "Столица");
			this.AddColumn("Population", "Численность населения");
		}
		
		protected override void formLoad(object sender, EventArgs e) 
		{	
			string strSql = @"select 
									id,
									Name,
									CountryCode,
									District,
									Population
								from city";
			
			MySqlCommand cmd = new MySqlCommand(strSql, this.conn);
			MySqlDataReader dataReader = cmd.ExecuteReader();
			
			this.tbl.Load(dataReader);
			this.DataSource = this.tbl;
		}
		
		protected override void btnCreate_Click(object sender, EventArgs e)
		{
			DataRow row = this.tbl.NewRow();
			CityEdit form = new CityEdit(row, "Добавить город", this.conn);
			
			if (form.ShowDialog() == DialogResult.OK) 
			{
				string sql = @"INSERT INTO City (
									Name,
									CountryCode,
									District,
									Population
								) VALUES (
									@Name,
									@CountryCode,
									@District,
									@Population);
								SELECT LAST_INSERT_ID();";
				
				MySqlCommand cmd = new MySqlCommand(sql, this.conn);
				cmd.Parameters.AddRange(this.CreateParameters(row));
				
				object id = cmd.ExecuteScalar();
				row.SetField<object>("id", id);
				this.tbl.Rows.Add(row);
			}
		}
		
		private MySqlParameter[] CreateParameters(DataRow row)
		{
			MySqlParameter[] par = new MySqlParameter[4];
			
			par[0] = new MySqlParameter("@Name", MySqlDbType.String, 35)
			{ Value = row.Field<string>("Name") };
			
			par[1] = new MySqlParameter("@CountryCode", MySqlDbType.String, 3)
			{ Value = row.Field<string>("CountryCode") };
			
			par[2] = new MySqlParameter("@District", MySqlDbType.String, 20)
			{ Value = row.Field<string>("District") };
			
			par[3] = new MySqlParameter("@Population", MySqlDbType.Int32, 11)
			{ Value = row.Field<Int32>("Population") };
			
			return par;
		}
		
		protected override void btnEdit_Click(object sender, EventArgs e)
		{
			if (this.tbl.Rows.Count > 0 && 
			    this.grid.CurrentRow.Index < this.tbl.Rows.Count) {}
			else return;
			
			DataRow row = this.tbl.Rows[this.grid.CurrentRow.Index]; 
			CityEdit form = new CityEdit(row, "Редактировать город", this.conn);
			
			if (form.ShowDialog() == DialogResult.OK)
			{
				string sql = @"UPDATE City SET
									Name = @Name,
									CountryCode = @CountryCode,
									District = @District,
									Population = @Population
								WHERE id = @id";
				
				MySqlCommand cmd = new MySqlCommand(sql, this.conn);
				cmd.Parameters.AddRange(this.CreateParameters(row));
				cmd.Parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32, 11)
				                   { Value = row.Field<int>("id") });
				
				if (cmd.ExecuteNonQuery() > 0)
					row.AcceptChanges();
				
			}
		}
		
		protected sealed override void btnDelete_Click(object sender, EventArgs e)
		{
			if (this.tbl.Rows.Count > 0 && 
			    this.grid.CurrentRow.Index < this.tbl.Rows.Count) {}
			else return;
			
			DataRow row = this.tbl.Rows[this.grid.CurrentRow.Index];
			
			if (MessageBox.Show("Удалить город?", 
			                    "Города", 
			                    MessageBoxButtons.YesNo, 
			                    MessageBoxIcon.Question) == DialogResult.Yes)
			{
				string sql = "DELETE FROM City WHERE id = @id";
				MySqlCommand cmd = new MySqlCommand(sql, this.conn);
				cmd.Parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32, 11)
				                   { Value = row.Field<Int32>("id") });
				cmd.ExecuteNonQuery();
				row.Delete();
			}
		}
	}
	
	class CityEdit : EditForm 
	{
		private DataRow row;
		private MySqlConnection conn;
		
		public CityEdit(DataRow row, string title, MySqlConnection conn) 
		{
			this.row = row;
			this.Title = title;
			this.conn = conn;
			
			this.AddColumn("Name", 
			               "Название", 
			               row.Field<string>("Name"), 
			               properties: new ColumnProperties { Mask = 'C'.Replicate(35) });
			
			this.AddColumn("CountryCode", 
				           "Код страны",
			               row.Field<string>("CountryCode"), 
			               this.selCountryCode, 
			               new ColumnProperties { Mask = "LLL", ReadOnly = true });
			
			this.AddColumn("Population", 
			               "Численность населения", 
			               row.IsNull("Population") ? 0 : row.Field<int>("Population"),
			               properties: new ColumnProperties { Minimum = 0, Maximum = Int32.MaxValue });
			
			this.AddColumn("District", 
			               "Столица", 
			               row.Field<string>("District"),
			               properties: new ColumnProperties { Mask = 'C'.Replicate(20) });
		}
		
		protected sealed override void btnOk_Click(object sender, EventArgs e) 
		{
			this.row["Name"] = this.GetValue("Name");
			this.row["CountryCode"] = this.GetValue("CountryCode");
			this.row["District"] = this.GetValue("District");
			this.row["Population"] = this.GetValue("Population");
			
			this.Close();
		}
		
		private void selCountryCode(object sender, EventArgs e ) 
		{
			SelCountryCode form = new SelCountryCode(this.conn);
			DataRow row = form.ShowDialog();
			
			if (row != null && !row.IsNull("Code"))
				this.SetValue("CountryCode", row["Code"]);
		}	
	}
	
	class SelCountryCode : SelectForm 
	{
		private MySqlConnection conn;
		private DataTable tbl;
		
		public SelCountryCode(MySqlConnection conn)
		{
			this.tbl = new DataTable("country");
			
			this.conn = conn;
			this.Title = "Выбор кода страны";
			this.DataSource = this.tbl;
			
			this.AddColumn("code", "Код");
			this.AddColumn("name", "Название", 170);
			this.AddColumn("continent", "Континент", 200);
		}
		
		protected override void formLoad(object sender, EventArgs e)
		{
			MySqlCommand cmd = new MySqlCommand(
				"select code, name, continent from country", this.conn);
			
			MySqlDataReader reader = cmd.ExecuteReader();
			this.tbl.Load(reader);
		}
	}
}
