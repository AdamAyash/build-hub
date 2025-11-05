import getpass
import os
import subprocess
from utils import prompt_with_default

class DatabaseConnectionStringBuilder:
	connectionStringEnvironmentVariablePrefix = "ConnectionStrings__"
	connectionString = ""

	def __init__(self, databaseKey, databaseName):
		self.databaseKey = databaseKey
		self.databaseName = databaseName

	def build(self):
		print(f"Building connection string for {self.databaseName}...")
		dataSource = input("Please, specify a data source: ")
		initialCatalog = input("Please, specify a catalog: ")
		userID = input("Please, specify a user: ")
		password = getpass.getpass("Please, specify a password: ")

		self.connectionString = f"Data Source={dataSource};Initial Catalog={initialCatalog};User ID={userID};Password={password};"
		self.__save_connection_string()
	
	def __save_connection_string(self):
		confirmation = prompt_with_default("Do you want to save the connection string to an environment variable? (Y/n)", "y")
		if(confirmation.lower() == "n"):
			exit()

		connectionStringEnvironmentVariableName = self.connectionStringEnvironmentVariablePrefix + self.databaseKey
		subprocess.run(['setx', connectionStringEnvironmentVariableName, self.connectionString])

