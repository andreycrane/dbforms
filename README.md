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
			// �������� ������� � ������� ��������� ������, ���������, ������ (�� ��������� 100px)
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
		// ���������� �������� ���������� �� ������� �� ������ "�������", "�������������", "�������"
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