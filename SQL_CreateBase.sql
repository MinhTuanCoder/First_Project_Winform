Create database QLQuanAn;
CREATE TABLE MENU_FOOD
(
  ID_Food char(10) NOT NULL,
  Name_Food nchar(50) ,
  Price_Food Int, 
  Describe nchar(50) ,

  PRIMARY KEY (ID_Food)
);

CREATE TABLE MENU_DRINK
(
  ID_Drink char(10) NOT NULL,
  Name_Drink nchar(50) ,
  Price_Drink INT ,
  PRIMARY KEY (ID_Drink)
);

CREATE TABLE STATISTICAL
(
  ID_STTC char(10) NOT NULL,
  Total_Revenue INT ,
  Total_Cost INT ,
  Total_Profit INT ,
  Times date ,
  PRIMARY KEY (ID_STTC)
);

CREATE TABLE EXSPENDABLES
(
	ID_Exspend char(10) not null,
  Name_material nchar(50) NOT NULL,
  Quantity INT ,
  Price INT ,
  Total INT ,
  Times date,
  ID_STTC char(10) NOT NULL,
  PRIMARY KEY (ID_Exspend,Times),
  FOREIGN KEY (ID_STTC) REFERENCES STATISTICAL(ID_STTC)
);

CREATE TABLE BILL
(
  ID_Bill char(10) NOT NULL,
  Name_table nchar(50),
  Total_Bill INT,
  Times Datetime ,
  ID_STTC char(10) NOT NULL,
  PRIMARY KEY (ID_Bill),
  FOREIGN KEY (ID_STTC) REFERENCES STATISTICAL(ID_STTC)
);

CREATE TABLE ORDER_FOOD
(

  ID_Bill char(10) NOT NULL,
  ID_Food char(10) NOT NULL,
  NOF INT ,
  PRIMARY KEY (ID_Bill, ID_Food),
  FOREIGN KEY (ID_Bill) REFERENCES BILL(ID_Bill),
  FOREIGN KEY (ID_Food) REFERENCES MENU_FOOD(ID_Food)
);

CREATE TABLE ORDER_DRINK
(
ID_Bill  char(10) NOT NULL,
  ID_Drink char(10) NOT NULL,
  NOD INT ,
  PRIMARY KEY (ID_Drink, ID_Bill),
  FOREIGN KEY (ID_Drink) REFERENCES MENU_DRINK(ID_Drink),
  FOREIGN KEY (ID_Bill) REFERENCES BILL(ID_Bill)
);

