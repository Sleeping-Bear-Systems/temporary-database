# temporary-database

## Environment Variables

These environment variables are used to connect to the databases used for testing and
contains a connection string.

* `SBS_TEST_SERVER_MYSQL`
* `SBS_TEST_SERVER_POSTGRES`

For example, use PowerShell to set the `SBS_TEST_SERVER_MYSQL` environment variable:

`[Environment]::SetEnvironmentVariable("SBS_TEST_SERVER_MYSQL", "Server=sever.local.net;Port=12345;Uid=root;Password=<strong password>;", "User")`
