/*
 * Created by SharpDevelop.
 * User: admin
 * Date: 28.08.2014
 * Time: 1:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data;
using DbForms;

namespace TestForms
{
	/// <summary>
	/// Description of CountryLanguage.
	/// </summary>
	public class LanguageBrowse : BrowseForm
	{
		private MySqlConnection conn;
		private DataTable tbl; 
		
		public LanguageBrowse(MySqlConnection conn)
		{
			this.conn = conn;
			this.tbl = new DataTable("contrylanguage");
			
			this.Title = "Языки стран";
			
			this.AddColumn("CountryCode", "Код страны");
			this.AddColumn("Language", "Язык", 150);
			this.AddColumn("IsOfficial", "Официальный");
			this.AddColumn("Percentage", "Процент");
		}
		
		protected override void formLoad(object sender, EventArgs e)
		{
			string sql = @"SELECT 
								CountryCode, 
								Language, 
								IsOfficial, 
								Percentage
							FROM countrylanguage";
			
			MySqlCommand cmd = new MySqlCommand(sql, this.conn);
			MySqlDataReader reader = cmd.ExecuteReader();
			
			this.tbl.Load(reader);
			this.DataSource = this.tbl;
		}
		
		protected sealed override void btnCreate_Click(object sender, EventArgs e)
		{
			DataRow row = this.tbl.NewRow();
			LanguageEdit form = new LanguageEdit(row, "Добавить язык", this.conn, false);
			if (form.ShowDialog() == DialogResult.OK)
			{
				string sql = @"select COUNT(*) as cnt 
									from countrylanguage 
									where CountryCode = @CountryCode 
											AND Language = @Language";
				
				MySqlCommand cmd = new MySqlCommand(sql, this.conn);
				cmd.Parameters.Add(new MySqlParameter("@CountryCode", MySqlDbType.VarChar, 3)
				                   { Value = row.Field<string>("CountryCode") });
				cmd.Parameters.Add(new MySqlParameter("@Language", MySqlDbType.VarChar, 30)
				                   { Value = row.Field<string>("Language") });
				
				
				if (Convert.ToInt32(cmd.ExecuteScalar()) > 0) {
					MessageBox.Show("В таблице уже есть запись с таким ключем!",
							"Языки стран",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning);
					return;
				}
				
				sql = @"insert into countrylanguage ( 
											CountryCode, 
											Language,
											IsOfficial,
											Percentage
										) values (
											@CountryCode,
											@Language,
											@IsOfficial,
											@Percentage)";
				
				cmd = new MySqlCommand(sql, this.conn);
				
				cmd.Parameters.AddRange(this.CreateParameters(row));
								
				if (cmd.ExecuteNonQuery() > 0) {
					this.tbl.Rows.Add(row);
				}
				
				
			}
		}
		
		protected sealed override void btnEdit_Click(object sender, EventArgs e)
		{
			DataRow row = this.tbl.Rows[this.grid.CurrentRow.Index];
			LanguageEdit form = new LanguageEdit(row, "Изменить язык");
			
			if(form.ShowDialog() == DialogResult.OK) {
				string sql = @"update countrylanguage set
									IsOfficial = @IsOfficial,
									Percentage = @Percentage
								where CountryCode = @CountryCode
									and Language = @Language";
				MySqlCommand cmd = new MySqlCommand(sql, this.conn);
				
				cmd.Parameters.AddRange(this.CreateParameters(row));
				if(cmd.ExecuteNonQuery() > 0)
					row.AcceptChanges();
			}
		}
		
		private MySqlParameter[] CreateParameters(DataRow row)
		{
			MySqlParameter[] par = new MySqlParameter[4];
			
			par[0] = new MySqlParameter("@CountryCode", MySqlDbType.VarChar, 3)
				{ Value = row.Field<string>("CountryCode") };
			par[1] = new MySqlParameter("@Language", MySqlDbType.VarChar, 30)
				{ Value = row.Field<string>("Language") };
			par[2] = new MySqlParameter("@IsOfficial", MySqlDbType.VarChar, 1)
				{ Value = row.Field<string>("IsOfficial") };
			par[3] = new MySqlParameter("@Percentage", MySqlDbType.Float, 4)
				{ Value = row.Field<float>("Percentage") };
			
			return par;
		}
		
		protected sealed override void btnDelete_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Удалить язык?", 
			                    	"Языки стран",
			                    	MessageBoxButtons.YesNo,
			                    	MessageBoxIcon.Question) == DialogResult.No)
				return;
			
			DataRow row = this.tbl.Rows[
				this.grid.CurrentRow.Index];
			
			string sql = @"delete from countrylanguage 
								where CountryCode = @CountryCode
										and Language = @Language";
			
			MySqlCommand cmd = new MySqlCommand(sql, this.conn);
			
			cmd.Parameters.Add(new MySqlParameter("@CountryCode", MySqlDbType.VarChar, 3)
			                   { Value = row.Field<string>("CountryCode") });
			cmd.Parameters.Add(new MySqlParameter("@Language", MySqlDbType.VarChar, 30)
			                   { Value = row.Field<string>("Language") });
		
			if (cmd.ExecuteNonQuery() > 0)
				row.Delete();
		}
	}
	
	class LanguageEdit : EditForm 
	{
		private DataRow row;
		private MySqlConnection conn;
		private bool IsEdit;
		
		public LanguageEdit(DataRow row, string title, MySqlConnection conn = null, bool IsEdit = true)
		{
			this.row = row;
			this.Title = title;
			this.conn = conn;
			this.IsEdit = IsEdit;
			
			if(!IsEdit) {
				this.AddColumn("CountryCode", 
				               "Код страны", 
				               row.Field<string>("CountryCode"), 
				               this.SelCntrCode, 
				               new ColumnProperties { ReadOnly = true });
				
				this.AddColumn("Language", 
				               "Язык", 
				               row.Field<string>("Language"),
				               properties: new ColumnProperties { Mask = 'C'.Replicate(30) });
			}
			
			bool isOfficial = false; 
			
			if (!row.IsNull("IsOfficial")) 
			{
				string isOffStr = row.Field<string>("IsOfficial");
				isOfficial = (isOffStr == "T");
			}
			
			this.AddColumn("IsOfficial", "Официальный", isOfficial);
			this.AddColumn("Percentage", 
			               "Процент", 
			               row.Field<float?>("Percentage") ?? 0.0f,
			               properties: new ColumnProperties { DecimalPlaces = 1, Minimum = 0, 
			               										Maximum = 100.0m, Increment = 0.1m });
		}
		
		protected sealed override void btnOk_Click(object sender, EventArgs e)
		{
			if (!this.IsEdit) {
				this.row["Language"] = this.GetValue("Language");
				this.row["CountryCode"] = this.GetValue("CountryCode");	
			}
			
			this.row["Percentage"] = this.GetValue("Percentage");
			this.row["IsOfficial"] = ((bool)this.GetValue("IsOfficial")) ? "T" : "F";
			
			this.Close();
		}
		
		protected void SelCntrCode(object sender, EventArgs e)
		{
			SelCountryCode form = new SelCountryCode(this.conn);
			DataRow row = form.ShowDialog();
			
			if (row != null)
			{
				this.SetValue("CountryCode", row.Field<string>("Code"));
			}
		}
	}
}
