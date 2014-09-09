Database Forms 
=================================

Набор базовых классов для .NET упрощающих разработку оконных приложений 
для работы с базами данных. Классы представленные в библиотеке упрощают
программирование Windows-форм для выполнения типичных CRUD-операций.
Данная библиотека не предоставляет средств для взаимеодействия с какой-либо
конкретной СУБД, т.к. предназначенна для использования с любым провайдером
данных: SQLite,  MySQL и т.д.

В библиотеке реализованы следующие базовые абстрактные классы:

* BrowseForm - форма просмотре и редактирования табличной информации
* EditForm - форма редактирования элемента таблицы
* SelectForm - форма выбора элемента табличной информации (справочник)
* Helpers - статический класс с несколькими расширениями

Ключевые идеи
---------------------------------

В большинстве случаев при создании Windows-приложений работающих с базами данных
приходится выполнять одну и ту же рутинную работу создания форм для отображения
табличных данных. Каждый раз в дизайнере или тем более вручную в 
программном коде мы добавляем элементы управления сетки, кнопок "добавить", "изменить", 
"удалить", текстового поля поиска по таблице и т.д. Кроме того нам приходится 
создавать еще и новую форму для редактирования-обновления этих данных. Каждая новая 
таблица (совокупность таблиц) в базе данных - это новый набор форм в приложении.

Этот небольшой проект, пусть даже и один из многих, призван решить эту проблему.
Каким именно образом, показано в примерах приведенных ниже, а также в проекте
включенном в данный репозиторий, под названием TestForms - представляющий
небольшое Windows-приложение работающее с тестовой базой данных World входящей
в поставку "MySQL 5.6".

Примеры
---------------------------------

####Пример №1


Задача: необходима форма для отображения табличных данных взятых из 
БД.

```csharp
	using System;
	using System.Data;
	using System.Windows.Forms;
	using MySql.Data.MySqlClient;
	using DbForms;
	
	// Наследуемся от базового класса табличной формы
	class CityBrowse : BrowseForm
	{
		// Объект подключения к СУБД и загружаемой таблицы
		private MySqlConnection conn;
		private DataTable tbl;
		
		// Определяем дочерний конструктор в котором настраиваем форму
		public CityBrowse(MySqlConnection conn)
		{
			this.conn = conn;
			this.tbl = new DataTable("city");
			
			// заголовок формы
			this.Title = "Города";
			// Список отображаемых столбцов
			// название столбца в объекте источника данных, 
			// заголовок, ширина (по умолчанию 100px)
			this.AddColumn("Name", "Название", 200);
			this.AddColumn("CountryCode", "Код страны");
			this.AddColumn("District", "Столица");
			this.AddColumn("Population", "Численность населения");
			// определяем кнопки дополнительных действий
			this.AddAction("Фильтр по стране", this.filterByCountry);
		}
		// переопределяем метод загрузки формы
		// реализуем в ней загрузку данных
		protected override void formLoad(object sender, EventArgs e) 
		{	
			// загружаем данные
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
			// определям источник данных отображаемых на форме
			this.DataSource = this.tbl;
		}
		// определяем действия выполняемы по нажатию 
		// на кнопки "Создать", "Редактировать", "Удалить"
		protected sealed override void btnCreate_Click(object sender, EventArgs e) {}
		protected sealed override void btnEdit_Click(object sender, EventArgs e) {}
		protected sealed override void btnDelete_Click(object sender, EventArgs e) {}
		// обработчик дополнительного действия фильтра по стране
		private void filterByCountry(object sender, EventArgs e) { }
	}
	
	CityBrowse form = new CityBrowse(conn);
	form.ShowDialog();
```

В результате получаем слeдующее:

![Изображение результрующей формы](http://firepic.org/images/2014-09/09/emscb2s8a8x3.png "Результирующая форма")

####Пример №2

Задача: Необходима форма для редактирования строки табличных данных. Одно из полей 
должно заполнятся данными из справочника.

```csharp

	// Реализуем абстрактный базовый класс EditForm
	class CityEdit : EditForm 
	{
		// Сылка на редактируемую форму
		// и объект соединения с БД необходимый для передачи справочнику
		private DataRow row;
		private MySqlConnection conn;
		// Определяем конструктор с тремя параметрами
		// Редактируемая строка, заголовок форма ("Добавить новую строку",
		// "Редактировать существующую"), объект соединения с БД
		public CityEdit(DataRow row, string title, MySqlConnection conn) 
		{
			this.row = row;
			this.Title = title;
			this.conn = conn;
			
			// определяем редактируемые поля
			this.AddColumn("Name", 						// Наименование столбца в строке
			               "Название", 					// Заголовок
			               row.Field<string>("Name"), 	// Начальное значение
			               properties: new ColumnProperties { 
			               		Mask = 'C'.Replicate(35) }); // Дополнительные свойства
							
			this.AddColumn("CountryCode", 
				           "Код страны",
			               row.Field<string>("CountryCode"), 
			               this.selCountryCode, 	// Обработчик щелча 
			               							// по ссылке (вызов выбора значения из справочника)
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
		// Переопределяем действие выполняемое по щелчку на кнопке "Ok"
		// Здесь можно реализовать валидацию данных
		// заполнения строки данными из добавленных на форму полей
		protected sealed override void btnOk_Click(object sender, EventArgs e) 
		{
			this.row["Name"] = this.GetValue("Name");
			this.row["CountryCode"] = this.GetValue("CountryCode");
			this.row["District"] = this.GetValue("District");
			this.row["Population"] = this.GetValue("Population");
			
			this.Close();
		}
		// Обработчик щелчка по заголовку-сылке поля (Выбор кода страны
		// из справочника)
		private void selCountryCode(object sender, EventArgs e) {}	
	}
	
	CityEdit form = new CityEdit(row, "Добавить Город", conn);
	form.ShowDialog();
```

В результате получаем:
![Если Вы видите это текст, значит фотохостер меня обманул :(](http://firepic.org/images/2014-09/09/xt73nwlh6naz.png "Форма редактирования записи БД")

####Пример №3

Задача: Нам необходима форма для выбора строки данных из некоторой таблицы
для заполнения поля значением первичного ключа из этой строки. Проще говоря
выбрать справочные данные.

```csharp
	// Реализуем абстрактный базовый класс SelectForm
	class SelCountryCode : SelectForm 
	{
		private MySqlConnection conn;
		private DataTable tbl;
		
		// Настраиваем форму в конструкторе дочернего класса
		public SelCountryCode(MySqlConnection conn)
		{
			this.tbl = new DataTable("country");
			
			this.conn = conn;
			// Заголовок
			this.Title = "Выбор кода страны";
			// Источник данных для формы
			this.DataSource = this.tbl;
			
			// Поля отображаемые на форме
			// Название поля в источнике данных
			// Заголовок
			// Опционально ширина поля
			this.AddColumn("code", "Код");
			this.AddColumn("name", "Название", 170);
			this.AddColumn("continent", "Континент", 200);
		}
		// переопределяем загрузку формы 
		protected override void formLoad(object sender, EventArgs e)
		{
			// Загружаем данные в источник
			MySqlCommand cmd = new MySqlCommand(
				"select code, name, continent from country", this.conn);
			
			MySqlDataReader reader = cmd.ExecuteReader();
			this.tbl.Load(reader);
		}
	}
	
	SelCountryCode form = new SelCountryCode(conn);
	// если пользователь потвердит выбор строки
	// метод вернет ссылку на выбранную строку
	// в простивном случае вернет null
	DataRow r = form.ShowDialog();
```

В результате получаем:
![Если здесь нет изображения формы, значит фотохостер меня обманул :(](http://firepic.org/images/2014-09/10/ymq56i3ti187.png "Форма выбора справочной информации")