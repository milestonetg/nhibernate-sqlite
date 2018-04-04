# MilestoneTG.NHibernate.Driver.Sqlite.Microsoft

A NHibernate SQLite driver using Microsoft.Data.Sqlite.

## Configuration

Example configuration template:

``` xml
<?xml version="1.0" encoding="utf-8"?>
<hibernate-configuration xmlns="urn:nhibernate-configuration-2.2" >
	<session-factory name="NHibernate.Test">
		<property name="connection.driver_class">MilestoneTG.NHibernate.Driver.Sqlite.Microsoft.MicrosoftSqliteDriver, MilestoneTG.NHibernate.Driver.Sqlite.Microsoft</property>
		<!-- DateTimeFormatString allows to prevent storing the fact that written date was having kind UTC,
		     which dodges the undesirable time conversion to local done on reads by System.Data.SQLite.
		     See https://system.data.sqlite.org/index.html/tktview/44a0955ea344a777ffdbcc077831e1adc8b77a36
		     and https://github.com/nhibernate/nhibernate-core/issues/1362 -->
		<property name="connection.connection_string">
			Data Source=nhibernate.db;
			DateTimeFormatString=yyyy-MM-dd HH:mm:ss.FFFFFFF;
		</property>
		<property name="dialect">NHibernate.Dialect.SQLiteDialect</property>
	</session-factory>
</hibernate-configuration>
```
