Database Forms 
=================================

����� ������� ������� ��� .NET ���������� ���������� ������� ���������� 
��� ������ � ������ ������. ������ �������������� � ���������� ��������
���������������� Windows-���� ��� ���������� �������� CRUD-��������.
������ ���������� �� ������������� ������� ��� ��������������� � �����-����
���������� ����, �.�. �������������� ��� ������������� � ����� �����������
������: SQLite,  MySQL � �.�.

� ���������� ����������� ��������� ������� ����������� ������:

* BrowseForm - ����� ��������� � �������������� ��������� ����������
* EditForm - ����� �������������� �������� �������
* SelectForm - ����� ������ �������� ��������� ���������� (����������)
* Helpers - ����������� ����� � ����������� ������������

�������� ����
---------------------------------

� ����������� ������� ��� �������� Windows-���������� ���������� � ������ ������
���������� ��������� ���� � �� �� �������� ������ �������� ���� ��� �����������
��������� ������. ������ ��� � ��������� ��� ��� ����� ������� � 
����������� ���� �� ��������� �������� ���������� �����, ������ "��������", "��������", 
"�������", ���������� ���� ������ �� ������� � �.�. ����� ���� ��� ���������� 
��������� ��� � ����� ����� ��� ��������������-���������� ���� ������. ������ ����� 
������� (������������ ������) � ���� ������ - ��� ����� ����� ���� � ����������.

���� ��������� ������, ����� ���� � ���� �� ������, ������� ������ ��� ��������.
����� ������ �������, �������� � �������� ����������� ����, � ����� � �������
���������� � ������ �����������, ��� ��������� TestForms - ��������������
��������� Windows-���������� ���������� � �������� ����� ������ World ��������
� �������� "MySQL 5.6".

�������
---------------------------------

####������ �1


������: ���������� ����� ��� ����������� ��������� ������ ������ �� 
��.

```csharp
	using System;
	using System.Data;
	using System.Windows.Forms;
	using MySql.Data.MySqlClient;
	using DbForms;
	
	// ����������� �� �������� ������ ��������� �����
	class CityBrowse : BrowseForm
	{
		// ������ ����������� � ���� � ����������� �������
		private MySqlConnection conn;
		private DataTable tbl;
		
		// ���������� �������� ����������� � ������� ����������� �����
		public CityBrowse(MySqlConnection conn)
		{
			this.conn = conn;
			this.tbl = new DataTable("city");
			
			// ��������� �����
			this.Title = "������";
			// ������ ������������ ��������
			// �������� ������� � ������� ��������� ������, 
			// ���������, ������ (�� ��������� 100px)
			this.AddColumn("Name", "��������", 200);
			this.AddColumn("CountryCode", "��� ������");
			this.AddColumn("District", "�������");
			this.AddColumn("Population", "����������� ���������");
			// ���������� ������ �������������� ��������
			this.AddAction("������ �� ������", this.filterByCountry);
		}
		// �������������� ����� �������� �����
		// ��������� � ��� �������� ������
		protected override void formLoad(object sender, EventArgs e) 
		{	
			// ��������� ������
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
			// ��������� �������� ������ ������������ �� �����
			this.DataSource = this.tbl;
		}
		// ���������� �������� ���������� �� ������� 
		// �� ������ "�������", "�������������", "�������"
		protected sealed override void btnCreate_Click(object sender, EventArgs e) {}
		protected sealed override void btnEdit_Click(object sender, EventArgs e) {}
		protected sealed override void btnDelete_Click(object sender, EventArgs e) {}
		// ���������� ��������������� �������� ������� �� ������
		private void filterByCountry(object sender, EventArgs e) { }
	}
	
	CityBrowse form = new CityBrowse(conn);
	form.ShowDialog();
```

� ���������� �������� ��e������:

![����������� ������������� �����](http://firepic.org/images/2014-09/09/emscb2s8a8x3.png "�������������� �����")

####������ �2

������: ���������� ����� ��� �������������� ������ ��������� ������. ���� �� ����� 
������ ���������� ������� �� �����������.

```csharp

	// ��������� ����������� ������� ����� EditForm
	class CityEdit : EditForm 
	{
		// ����� �� ������������� �����
		// � ������ ���������� � �� ����������� ��� �������� �����������
		private DataRow row;
		private MySqlConnection conn;
		// ���������� ����������� � ����� �����������
		// ������������� ������, ��������� ����� ("�������� ����� ������",
		// "������������� ������������"), ������ ���������� � ��
		public CityEdit(DataRow row, string title, MySqlConnection conn) 
		{
			this.row = row;
			this.Title = title;
			this.conn = conn;
			
			// ���������� ������������� ����
			this.AddColumn("Name", 						// ������������ ������� � ������
			               "��������", 					// ���������
			               row.Field<string>("Name"), 	// ��������� ��������
			               properties: new ColumnProperties { 
			               		Mask = 'C'.Replicate(35) }); // �������������� ��������
							
			this.AddColumn("CountryCode", 
				           "��� ������",
			               row.Field<string>("CountryCode"), 
			               this.selCountryCode, 	// ���������� ����� 
			               							// �� ������ (����� ������ �������� �� �����������)
			               new ColumnProperties { Mask = "LLL", ReadOnly = true });
			
			this.AddColumn("Population", 
			               "����������� ���������", 
			               row.IsNull("Population") ? 0 : row.Field<int>("Population"),
			               properties: new ColumnProperties { Minimum = 0, Maximum = Int32.MaxValue });
			
			this.AddColumn("District", 
			               "�������", 
			               row.Field<string>("District"),
			               properties: new ColumnProperties { Mask = 'C'.Replicate(20) });
		}
		// �������������� �������� ����������� �� ������ �� ������ "Ok"
		// ����� ����� ����������� ��������� ������
		// ���������� ������ ������� �� ����������� �� ����� �����
		protected sealed override void btnOk_Click(object sender, EventArgs e) 
		{
			this.row["Name"] = this.GetValue("Name");
			this.row["CountryCode"] = this.GetValue("CountryCode");
			this.row["District"] = this.GetValue("District");
			this.row["Population"] = this.GetValue("Population");
			
			this.Close();
		}
		// ���������� ������ �� ���������-����� ���� (����� ���� ������
		// �� �����������)
		private void selCountryCode(object sender, EventArgs e) {}	
	}
	
	CityEdit form = new CityEdit(row, "�������� �����", conn);
	form.ShowDialog();
```

� ���������� ��������:
![���� �� ������ ��� �����, ������ ���������� ���� ������� :(](http://firepic.org/images/2014-09/09/xt73nwlh6naz.png "����� �������������� ������ ��")

####������ �3

������: ��� ���������� ����� ��� ������ ������ ������ �� ��������� �������
��� ���������� ���� ��������� ���������� ����� �� ���� ������. ����� ������
������� ���������� ������.

```csharp
	// ��������� ����������� ������� ����� SelectForm
	class SelCountryCode : SelectForm 
	{
		private MySqlConnection conn;
		private DataTable tbl;
		
		// ����������� ����� � ������������ ��������� ������
		public SelCountryCode(MySqlConnection conn)
		{
			this.tbl = new DataTable("country");
			
			this.conn = conn;
			// ���������
			this.Title = "����� ���� ������";
			// �������� ������ ��� �����
			this.DataSource = this.tbl;
			
			// ���� ������������ �� �����
			// �������� ���� � ��������� ������
			// ���������
			// ����������� ������ ����
			this.AddColumn("code", "���");
			this.AddColumn("name", "��������", 170);
			this.AddColumn("continent", "���������", 200);
		}
		// �������������� �������� ����� 
		protected override void formLoad(object sender, EventArgs e)
		{
			// ��������� ������ � ��������
			MySqlCommand cmd = new MySqlCommand(
				"select code, name, continent from country", this.conn);
			
			MySqlDataReader reader = cmd.ExecuteReader();
			this.tbl.Load(reader);
		}
	}
	
	SelCountryCode form = new SelCountryCode(conn);
	// ���� ������������ ��������� ����� ������
	// ����� ������ ������ �� ��������� ������
	// � ���������� ������ ������ null
	DataRow r = form.ShowDialog();
```

� ���������� ��������:
![���� ����� ��� ����������� �����, ������ ���������� ���� ������� :(](http://firepic.org/images/2014-09/10/ymq56i3ti187.png "����� ������ ���������� ����������")