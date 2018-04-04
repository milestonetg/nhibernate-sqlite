using NHibernate.Driver;
using NHibernate.Engine;
using System.Data;
using System.Data.Common;

namespace MilestoneTG.NHibernate.Driver.Sqlite.Microsoft
{
    /// <summary>
    /// NHibernate driver for the Microsoft.Data.SQLite data provider for .NET.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In order to use this driver you must have the Microsoft.Data.SQLite.dll assembly available
    /// for NHibernate to load. 
    /// </para>
    /// <para>
    /// You can get the Microsoft.Data.SQLite.dll assembly from NuGet:
    /// <a href="https://www.nuget.org/packages/Microsoft.Data.SQLite/">https://www.nuget.org/packages/Microsoft.Data.SQLite/</a>
    /// </para>
    /// <para>
    /// Please check <a href="https://www.sqlite.org/">https://www.sqlite.org/</a> for more information regarding SQLite.
    /// </para>
    /// </remarks>
    public class MicrosoftSqliteDriver : ReflectionBasedDriver
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MicrosoftSqliteDriver"/>.
        /// </summary>
        /// <exception cref="HibernateException">
        /// Thrown when the <c>SQLite.NET</c> assembly can not be loaded.
        /// </exception>
        public MicrosoftSqliteDriver() : base(
            "Microsoft.Data.Sqlite",
            "Microsoft.Data.Sqlite",
            "Microsoft.Data.Sqlite.SqliteConnection",
            "Microsoft.Data.Sqlite.SqliteCommand")
        {
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <returns>DbConnection.</returns>
        public override DbConnection CreateConnection()
        {
            var connection = base.CreateConnection();
            connection.StateChange += Connection_StateChange;
            return connection;
        }

        private static void Connection_StateChange(object sender, StateChangeEventArgs e)
        {
            if ((e.OriginalState == ConnectionState.Broken || e.OriginalState == ConnectionState.Closed || e.OriginalState == ConnectionState.Connecting) &&
                e.CurrentState == ConnectionState.Open)
            {
                var connection = (DbConnection)sender;
                using (var command = connection.CreateCommand())
                {
                    // Activated foreign keys if supported by SQLite.  Unknown pragmas are ignored.
                    command.CommandText = "PRAGMA foreign_keys = ON";
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Gets the result sets command.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns>IResultSetsCommand.</returns>
        public override IResultSetsCommand GetResultSetsCommand(ISessionImplementor session)
        {
            return new BasicResultSetsCommand(session);
        }

        /// <summary>
        /// Does this Driver require the use of a Named Prefix in the SQL statement.
        /// </summary>
        /// <value><c>true</c> if [use named prefix in SQL]; otherwise, <c>false</c>.</value>
        /// <remarks>For example, SqlClient requires <c>select * from simple where simple_id = @simple_id</c>
        /// If this is false, like with the OleDb provider, then it is assumed that
        /// the <c>?</c> can be a placeholder for the parameter in the SQL statement.</remarks>
        public override bool UseNamedPrefixInSql => true;

        /// <summary>
        /// Does this Driver require the use of the Named Prefix when trying
        /// to reference the Parameter in the Command's Parameter collection.
        /// </summary>
        /// <value><c>true</c> if [use named prefix in parameter]; otherwise, <c>false</c>.</value>
        /// <remarks>This is really only useful when the UseNamedPrefixInSql == true.  When this is true the
        /// code will look like:
        /// <code>DbParameter param = cmd.Parameters["@paramName"]</code>
        /// if this is false the code will be
        /// <code>DbParameter param = cmd.Parameters["paramName"]</code>.</remarks>
        public override bool UseNamedPrefixInParameter => true;

        /// <summary>
        /// The Named Prefix for parameters.
        /// </summary>
        /// <value>The named prefix.</value>
        /// <remarks>Sql Server uses <c>"@"</c> and Oracle uses <c>":"</c>.</remarks>
        public override string NamedPrefix => "@";

        /// <summary>
        /// Gets a value indicating whether [supports multiple open readers].
        /// </summary>
        /// <value><c>true</c> if [supports multiple open readers]; otherwise, <c>false</c>.</value>
        public override bool SupportsMultipleOpenReaders => false;

        /// <summary>
        /// Gets a value indicating whether [supports multiple queries].
        /// </summary>
        /// <value><c>true</c> if [supports multiple queries]; otherwise, <c>false</c>.</value>
        public override bool SupportsMultipleQueries => true;

        /// <summary>
        /// Gets a value indicating whether [supports null enlistment].
        /// </summary>
        /// <value><c>true</c> if [supports null enlistment]; otherwise, <c>false</c>.</value>
        public override bool SupportsNullEnlistment => false;

        /// <summary>
        /// Gets a value indicating whether this instance has delayed distributed transaction completion.
        /// </summary>
        /// <value><c>true</c> if this instance has delayed distributed transaction completion; otherwise, <c>false</c>.</value>
        public override bool HasDelayedDistributedTransactionCompletion => true;
    }
}

