About:	ConsolidatedDBTransactions Application
	Brian Hughes
	2008/05/13

This is just a quick and dirty desktop application that allows the user to
create and persist DataSet schema files (.xsd) and DataSets with the transactions
from a selected ConsolidatedDB database. The contents of these files (both the
schema and the data) are "mostly" created based on the ConsolidatedDB database
selected by the application user. "Mostly" because the tables, columns, primary
keys, ... are created based on the actual database schema. But the foreign key
constraints are created in code and therfore are hard code into the application.

This program was created because I wanted a simple way to create "up-to-date"
.xsd files as well as sample data that would exist in a DataSet with that schema.