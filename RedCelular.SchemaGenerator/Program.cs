// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="RedCelular">
//   2011
// </copyright>
// <summary>
//   The entry point for the project.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedCelular.SchemaGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Castle.ActiveRecord;
    using Castle.ActiveRecord.Framework.Config;

    using NHibernate;

    using RedCelular.Framework;
    using RedCelular.Framework.Catalogs;

    /// <summary>
    /// The entry point for the project.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Backing field for Connection property.
        /// </summary>
        private static IDbConnection __connection;

        /// <summary>
        /// Session scope for the application.
        /// </summary>
        private static SessionScope __sessionScope;

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        private static IDbConnection Connection
        {
            get
            {
                if (__connection == null)
                {
                    ISession session = ActiveRecordMediator.GetSessionFactoryHolder().GetSessionFactory(typeof(Product)).OpenSession();
                    __connection = session.Connection;
                }

                return __connection;
            }
        }

        /// <summary>
        /// Mains the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            string option;
            if (args.Length == 0)
            {
                Console.WriteLine("Please specify a valid option...");
                Console.WriteLine("Valid parameter values are: -update|-bootstrap|-tests [-show_workflow]");
                option = Console.ReadLine();
            }
            else
            {
                option = args[0];
            }

            Console.WriteLine("Initializing framework...");
            var source = new XmlConfigurationSource(@"SchemaConfig.xml");
            RedCelularActiveRecordBase<Product>.Initialize(source);
            __sessionScope = new SessionScope();
            switch (option)
            {
                case "-update":
                    UpdateSchema();
                    break;
                case "-bootstrap":
                    BootstrapDatabase();
                    break;
                case "-fill": // The database needs to be already created (bootstrap)
                    FillDatabase();
                    break;
                case "-tests":
                    PrepareTestsDatabase();
                    break;
                default:
                    Console.WriteLine("Valid parameter values are: -update|-bootstrap|-tests [-show_workflow]");
                    Console.ReadKey();
                    return;
            }

            __sessionScope.Flush();
        }


        /// <summary>
        /// Fills the database after being created (bootstrap).
        /// </summary>
        private static void FillDatabase()
        {
            Console.WriteLine("Populating DB...");
            StringBuilder query = new StringBuilder();
            StreamWriter sw = new StreamWriter("execute.sql");

            StreamReader reader = new StreamReader("NOCHECK-Contraints.sql");
            query.AppendLine(reader.ReadToEnd());
            sw.Write(reader.ReadToEnd());

            reader.BaseStream.Position = 0;

            reader.ReadLine();
            sw.WriteLine(reader.ReadToEnd());
            reader = new StreamReader("FillDataBase.sql");
            query.AppendLine(reader.ReadToEnd());
            reader = new StreamReader("CHECK-Contraints.sql");
            query.AppendLine(reader.ReadToEnd());

            ExecuteQuery(query.ToString());
            Console.WriteLine("Process successfully completed!");
        }

        /// <summary>
        /// Bootstraps the database.
        /// </summary>
        private static void BootstrapDatabase()
        {
            Console.WriteLine("Recreating DB...");
            Clear();
            Console.WriteLine("Bootstrapping DB...");
            CreateDatabaseObjects();
            StringBuilder query = new StringBuilder();
            StreamReader reader = new StreamReader("NOCHECK-Contraints.sql");
            query.AppendLine(reader.ReadToEnd());
            reader = new StreamReader("RedCelular.sql");
            query.AppendLine(reader.ReadToEnd());
            reader = new StreamReader("CHECK-Contraints.sql");
            query.AppendLine(reader.ReadToEnd());
            ExecuteQuery(query.ToString());
            Console.WriteLine("Process successfully completed!");
        }

        /// <summary>
        /// Clears the comcast configuration.
        /// </summary>
        /// <remarks>Deletes all tables from database</remarks>
        private static void Clear()
        {
            ActiveRecordStarter.DropSchema();
            DropTables();
            DropViews();
            ActiveRecordStarter.CreateSchema();
            /*ExecuteQuery("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PaperlessFetchRandomRecord]') AND type in (N'U')) DROP TABLE [dbo].[PaperlessFetchRandomRecord]");
            ExecuteQuery("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PaperlessFetchScheduledTask]') AND type in (N'U')) DROP TABLE [dbo].[PaperlessFetchScheduledTask]");
            ExecuteQuery("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PaperlessFetchNextRecord]') AND type in (N'U')) DROP TABLE [dbo].[PaperlessFetchNextRecord]");
            ExecuteQuery("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PaperlessFetchAbandonedTask]') AND type in (N'U')) DROP TABLE [dbo].[PaperlessFetchAbandonedTask]");*/
        }

        /// <summary>
        /// Drops the views.
        /// </summary>
        private static void DropViews()
        {
            const string Query = @"SELECT QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME) AS name FROM INFORMATION_SCHEMA.VIEWS";
            IDbCommand command = Connection.CreateCommand();
            command.CommandText = Query;
            IDataReader dataReader = command.ExecuteReader();
            List<string> dropQueries = new List<string>();
            while (dataReader.Read())
            {
                string viewName = dataReader.GetString(0);
                dropQueries.Add("DROP VIEW " + viewName);
            }

            dataReader.Close();
            dataReader.Dispose();
            foreach (string dropQuery in dropQueries)
            {
                ExecuteQuery(dropQuery);
            }
        }

        /// <summary>
        /// Drops the constraints.
        /// </summary>
        private static void DropTables()
        {
            const string Query = @"SELECT 'ALTER TABLE ' + TABLE_SCHEMA + '.' + TABLE_NAME + ' DROP CONSTRAINT ' + CONSTRAINT_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_TYPE = 'FOREIGN KEY'";
            IDbCommand command = Connection.CreateCommand();
            command.CommandText = Query;
            IDataReader dataReader = command.ExecuteReader();
            List<string> dropQueries = new List<string>();
            while (dataReader.Read())
            {
                string dropQuery = dataReader.GetString(0);
                dropQueries.Add(dropQuery);
            }

            dataReader.Close();
            dataReader.Dispose();
            foreach (string query in dropQueries)
            {
                ExecuteQuery(query);
            }

            ExecuteQuery("EXEC sp_MSforeachtable @command1 = \"DROP TABLE ?\"");
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="query">The query.</param>
        private static void ExecuteQuery(string query)
        {
            IDbCommand command = Connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = query;
            command.CommandTimeout = 200;
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates the database objects.
        /// </summary>
        private static void CreateDatabaseObjects()
        {
            List<string> files = Directory.GetFiles(".", "*.Entity.sql").ToList();
            ExecuteScripts(files);
        }

        /// <summary>
        /// Executes the scripts.
        /// </summary>
        /// <param name="files">The files containing the scripts.</param>
        private static void ExecuteScripts(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                Console.WriteLine("Executing script: " + file + "...");
                StreamReader reader = new StreamReader(file);
                ExecuteQuery(reader.ReadToEnd());
            }
        }

        /// <summary>
        /// Updates the schema.
        /// </summary>
        private static void UpdateSchema()
        {
            Console.WriteLine("Updating database schema...");
            ActiveRecordStarter.UpdateSchema();
            CreateDatabaseObjects();
            Console.WriteLine("Process successfully completed!");
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        private static void PrepareTestsDatabase()
        {
            BootstrapDatabase();
            FillDatabase();
        }
    }
}
