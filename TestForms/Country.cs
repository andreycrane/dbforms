/*
 * Created by SharpDevelop.
 * User: admin
 * Date: 25.08.2014
 * Time: 22:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient;
using DbForms;

namespace TestForms
{
	/// <summary>
	/// Description of CountryForm.
	/// </summary>
	public class CountryBrowse : BrowseForm
	{
		private MySqlConnection conn;
		private DataTable tbl;
		
		public CountryBrowse(MySqlConnection conn)
		{
			this.conn = conn;
			this.tbl = new DataTable("country");
			this.Title = "Страны";
			
			this.AddColumn("Code", "Код", 70);
			this.AddColumn("Name", "Название", 150);
			this.AddColumn("Continent", "Континент", 150);
			this.AddColumn("Region", "Регион", 150);
			this.AddColumn("SurfaceArea", "Площадь поверхности");
			this.AddColumn("IndepYear", "Год основания");
			this.AddColumn("Population", "Численность населения");
			this.AddColumn("LifeExpectancy", "Продолжительность жизни");
			this.AddColumn("GNP", "GNP");
			this.AddColumn("GNPOld", "GNPOld");
			this.AddColumn("LocalName", "В местном произношении");
			this.AddColumn("GovernmentForm", "Форма правления");
			this.AddColumn("HeadOfState", "Глава правительства");
			this.AddColumn("CapitalName", "Столица");
			this.AddColumn("Code2", "Второй код");
			
			this.AddAction("Языки", (sender, e) => {
			    DataRow row = this.tbl.Rows[this.grid.CurrentRow.Index];
			    CountryLanguages form = new CountryLanguages(this.conn, row);
			    form.ShowDialog();
			});
			
			this.AddAction("Города", (sender, e) => {
				DataRow row = this.tbl.Rows[this.grid.CurrentRow.Index];
				CountryCitys form = new CountryCitys(this.conn, row);
				form.ShowDialog();
			});
		}
		
		protected sealed override void formLoad(object sender, EventArgs e)
		{
			string sql = @"select 
								Code, 						-- Код страны
								country.Name, 				-- Название
								Continent, 					-- Континент расположения
								Region, 					-- Регион
								SurfaceArea, 				-- Площадь поверхности
								IndepYear, 					-- Год основание
								Country.Population, 		-- Численность населения
								LifeExpectancy, 			-- Продолжительность жизни
								GNP, 	
								GNPOld, 
								LocalName, 					-- Произношение в местной среде (в стране)
								GovernmentForm, 			-- Форма правления
								HeadOfState, 				-- Глава правительства
								Capital, 					-- Ссылка на столицу
								city.name AS CapitalName,	-- Название столицы
								Code2 						-- Второй код страны
							from country
							left join city
								on city.id = country.capital";
			
			MySqlCommand cmd = new MySqlCommand(sql, this.conn);
			MySqlDataReader dataReader = cmd.ExecuteReader();
			
			this.tbl.Load(dataReader);
			this.DataSource = this.tbl;
		}
		
		protected sealed override void btnCreate_Click(object sender, EventArgs e)
		{
			DataRow row = this.tbl.NewRow();
			EditCountry form = new EditCountry(row, "Создать запись", this.conn);
			
			if (form.ShowDialog() == DialogResult.OK) {
				string sql = @"INSERT INTO country
										(Code, 
										 Name, 
										 Continent, 
										 Region, 
										 SurfaceArea, 
										 IndepYear, 
										 Population, 
										 LifeExpectancy, 
										 GNP, 
										 GNPOld, 
										 LocalName, 
										 GovernmentForm, 
										 HeadOfState, 
										 Capital, 
										 Code2
									) VALUES (
										 @Code, 
										 @Name, 
										 @Continent, 
										 @Region, 
									     @SurfaceArea, 
										 @IndepYear,
										 @Population, 
										 @LifeExpectancy, 
										 @GNP, 
										 @GNPOld, 
									     @LocalName, 
										 @GovernmentForm, 
										 @HeadOfState, 
										 @Capital,
									     @Code2)";
				
				MySqlCommand cmd = new MySqlCommand(sql, this.conn);
				cmd.Parameters.AddRange(this.CreateParameters(row));
				
				if (cmd.ExecuteNonQuery() > 0) {
					this.tbl.Rows.Add(row);
					row.AcceptChanges();
				}
			}
		}
		
		protected sealed override void btnEdit_Click(object sender, EventArgs e)
		{
			DataRow row = this.tbl.Rows[this.grid.CurrentRow.Index];
			
			EditCountry form = new EditCountry(row, "Редактировать страну", this.conn, true);
			if(form.ShowDialog() == DialogResult.Cancel) return;
			
			string sql = @"UPDATE country SET
								Name=@Name,
								Continent=@Continent,
								Region=@Region,
								SurfaceArea=@SurfaceArea,
								IndepYear=@IndepYear,
								Population=@Population,
								LifeExpectancy=@LifeExpectancy,
								GNP=@GNP,
								GNPOld=@GNPOld,
								LocalName=@LocalName,
								GovernmentForm=@GovernmentForm,
								HeadOfState=@HeadOfState,
								Capital=@Capital,
								Code2=@Code2
							WHERE Code=@Code";
			MySqlCommand cmd = new MySqlCommand(sql, this.conn);
			cmd.Parameters.AddRange(this.CreateParameters(row));
			
			if (cmd.ExecuteNonQuery() > 0) row.AcceptChanges();
		}
		
		protected sealed override void btnDelete_Click(object sender, EventArgs e)
		{
			if(MessageBox.Show("Удалить строку", 
			                   "Страны",
			                   MessageBoxButtons.YesNo,
			                   MessageBoxIcon.Question) == DialogResult.No)
				return;
			
			
			DataRow row = this.tbl.Rows[this.grid.CurrentRow.Index];
			string sql = @"delete from country where code = @code";
			MySqlCommand cmd = new MySqlCommand(sql, this.conn);
			
			cmd.Parameters.Add(new MySqlParameter("@code", MySqlDbType.VarChar, 3)
			                   { Value = row.Field<string>("Code") });
			
			if (cmd.ExecuteNonQuery() > 0) row.Delete();
		}
		
		private MySqlParameter[] CreateParameters(DataRow row)
		{
			MySqlParameter[] param = new MySqlParameter[15];
			
			param[0] = new MySqlParameter("@Code", MySqlDbType.VarChar, 3)
			{ Value = row.Field<string>("Code") };
			
			param[1] = new MySqlParameter("@Name", MySqlDbType.VarChar, 52)
			{ Value = row.Field<string>("Name") };
			
			param[2] = new MySqlParameter("@Continent", MySqlDbType.VarChar, 20)
			{ Value = row.Field<string>("Continent") };
			
			param[3] = new MySqlParameter("@Region", MySqlDbType.VarChar, 26)
			{ Value = row.Field<string>("Region") };
			
			param[4] = new MySqlParameter("@SurfaceArea", MySqlDbType.Float, 10)
			{ Value = row.Field<float>("SurfaceArea") };
			
			param[5] = new MySqlParameter("@IndepYear", MySqlDbType.Int16, 6)
			{ Value = row.Field<short>("IndepYear") };
			
			param[6] = new MySqlParameter("@Population", MySqlDbType.Int32, 11)
			{ Value = row.Field<int>("Population") };
			
			param[7] = new MySqlParameter("@LifeExpectancy", MySqlDbType.Float, 3)
			{ Value = row.Field<float>("LifeExpectancy") };
			
			param[8] = new MySqlParameter("@GNP", MySqlDbType.Float, 10)
			{ Value = row.Field<float>("GNP") };
			
			param[9] = new MySqlParameter("@GNPOld", MySqlDbType.Float, 10)
			{ Value = row.Field<float>("GNPOld") };
			
			param[10] = new MySqlParameter("@LocalName", MySqlDbType.VarChar, 45)
			{ Value = row.Field<string>("LocalName") };
			
			param[11] = new MySqlParameter("@GovernmentForm", MySqlDbType.VarChar, 45)
			{ Value = row.Field<string>("GovernmentForm") };
			
			param[12] = new MySqlParameter("@HeadOfState", MySqlDbType.VarChar, 60)
			{ Value = row.Field<string>("HeadOfState") };
			
			param[13] = new MySqlParameter("@Capital", MySqlDbType.Int32, 11)
			{ Value = row.Field<int>("Capital") };
			
			param[14] = new MySqlParameter("@Code2", MySqlDbType.VarChar, 2)
			{ Value = row.Field<string>("Code2") };
			
			return param;
		}
	}
	
	class EditCountry : EditForm 
	{
		private DataRow row;
		private MySqlConnection conn;
		
		public EditCountry(DataRow row, string title, MySqlConnection conn, bool IsEdit = false)
		{
			this.row = row;
			this.Title = title;
			this.conn = conn;
			
			this.AddColumn("Code", 
			               "Код", 
			               this.row.Field<string>("Code"),
			               properties: new ColumnProperties { Mask = 'C'.Replicate(3), ReadOnly = IsEdit });
			
			this.AddColumn("Name", 
			               "Название", 
			               this.row.Field<string>("Name"),
			               properties: new ColumnProperties { Mask = 'C'.Replicate(52) });
			
			this.AddColumn("Continent", 
			               "Континент", 
			               this.row.Field<string>("Continent"), 
			               this.selCountryContinent,
			               new ColumnProperties { ReadOnly = true });
			
			this.AddColumn("Region", 
			               "Регион", 
			               this.row.Field<string>("Region"),
			               properties: new ColumnProperties { Mask = 'C'.Replicate(26) });
			
			this.AddColumn("SurfaceArea", 
			               "Площадь поверхности", 
			               this.row.Field<float?>("SurfaceArea") ?? 0.0f,
			               properties: new ColumnProperties { 
			               	Increment = 0.1m, 
			               	Minimum = 0,
			               	Maximum = 99999999m, 
			               	DecimalPlaces = 2 
			               });
			
			this.AddColumn("IndepYear", 
			               "Год основания", 
			               this.row.IsNull("IndepYear") ? 0 : this.row.Field<short>("IndepYear"),
			               properties: new ColumnProperties { Minimum = 0, Maximum = 3000 });
			
			this.AddColumn("Population", 
			               "Численность населения", 
			               row.Field<int?>("Population") ?? 0,
			               properties: new ColumnProperties { Minimum = 0, Maximum = Int32.MaxValue });
			
			this.AddColumn("LifeExpectancy", 
			               "Продолжительность жизни", 
			               row.Field<float?>("LifeExpectancy") ?? 0.0f,
			               properties: new ColumnProperties { Minimum = 0, Maximum = 100, 
			               				DecimalPlaces = 1, Increment = 0.1m });
			
			this.AddColumn("GNP",
			               "GNP",
			               row.Field<float?>("GNP") ?? 0.0f,
			               properties: new ColumnProperties { Minimum = 0, Maximum = 99999999.99m,
					               		DecimalPlaces = 2, Increment = 0.01m });
			
			this.AddColumn("GNPOld",
			               "GNPOld",
			               row.Field<float?>("GNP") ?? 0.0f,
			               properties: new ColumnProperties { Minimum = 0, Maximum = 99999999.99m,
					               		DecimalPlaces = 2, Increment = 0.01m });
			
			this.AddColumn("LocalName", 
			               "Местное произношение", 
			               row.Field<string>("LocalName"),
			               properties: new ColumnProperties { Mask = 'C'.Replicate(45) });
			
			this.AddColumn("GovernmentForm", 
			               "Форма правления", 
			               row.Field<string>("GovernmentForm"),
			               properties: new ColumnProperties { Mask = 'C'.Replicate(45) });
			
			this.AddColumn("HeadOfState", 
			               "Глава правительства", 
			               row.Field<string>("HeadOfState"),
			               properties: new ColumnProperties { Mask = 'C'.Replicate(60) });
			
			this.AddColumn("CapitalName", 
			               "Столица", 
			               row.Field<string>("CapitalName"),
			               this.selCountryCapital,
			               new ColumnProperties { ReadOnly = true });
			
			this.AddColumn("Code2", 
			               "Второй код", 
			               row.Field<string>("Code2"),
			               properties: new ColumnProperties { Mask = "CC" });
			
			this.AddHiddenColumn("Capital", row.Field<int?>("Capital") ?? 0);
		}
		
		private void selCountryContinent(object sender, EventArgs e)
		{
			SelectContinent form = new SelectContinent();
			DataRow row = form.ShowDialog();
			
			if (row != null)
				this.SetValue("Continent", row.Field<string>("Name"));
		}
		
		private void selCountryCapital(object sender, EventArgs e)
		{
			SelectCity form = new SelectCity(this.conn);
			DataRow row = form.ShowDialog();
			
			if (row != null) {
				this.SetValue("CapitalName", row.Field<string>("Name"));
				this.SetValue("Capital", row.Field<int>("id"));
			}
				                                               
		}
		
		protected sealed override void btnOk_Click(object sender, EventArgs e)
		{
			this.row["Code"] = this.GetValue("Code");
			this.row["Name"] = this.GetValue("Name");
			this.row["Continent"] = this.GetValue("Continent");
			this.row["Region"] = this.GetValue("Region");
			this.row["SurfaceArea"] = this.GetValue("SurfaceArea");
			this.row["IndepYear"] = this.GetValue("IndepYear");
			this.row["Population"] = this.GetValue("Population");
			this.row["LifeExpectancy"] = this.GetValue("LifeExpectancy");
			this.row["GNP"] = this.GetValue("GNP");
			this.row["GNPOld"] = this.GetValue("GNPOld");
			this.row["LocalName"] = this.GetValue("LocalName");
			this.row["GovernmentForm"] = this.GetValue("GovernmentForm");
			this.row["HeadOfState"] = this.GetValue("HeadOfState");
			this.row["Capital"] = this.GetValue("Capital");
			this.row["Code2"] = this.GetValue("Code2");
		}
	}
	
	class SelectContinent : SelectForm 
	{
		private DataTable tbl;
		
		public SelectContinent()
		{
			this.Title = "Континенты";
			
			this.tbl = new DataTable("Continents");
			this.DataSource = this.tbl;
			
			this.AddColumn("Name", "Название континента", 300);
		}
		
		protected override void formLoad(object sender, EventArgs e)
		{
			this.tbl.Columns.Add("Name", typeof(string));
			
			this.tbl.Rows.Add("Asia");
			this.tbl.Rows.Add("Europe");
			this.tbl.Rows.Add("North America");
			this.tbl.Rows.Add("Africa");
			this.tbl.Rows.Add("Oceania");
			this.tbl.Rows.Add("Antarctica");
			this.tbl.Rows.Add("South America");
		}
	}
	
	class SelectCity : SelectForm 
	{
		private MySqlConnection conn;
		private DataTable tbl;
		
		public SelectCity(MySqlConnection conn)
		{
			this.conn = conn;
			this.Title = "Города";
			this.tbl = new DataTable("city");
			
			this.DataSource = this.tbl;
			
			this.AddColumn("Name", "Название города", 300);
		}
		
		protected override void formLoad(object sender, EventArgs e)
		{
			string sql = "select id, name from city";
			MySqlCommand cmd = new MySqlCommand(sql, this.conn);
			
			
			MySqlDataReader reader = cmd.ExecuteReader();
			this.tbl.Load(reader);
		}
	}
}
