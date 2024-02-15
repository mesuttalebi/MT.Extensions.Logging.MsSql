# MT.Extensions.Logging.MsSql
A .net Core Logger Extension that logs to MsSql server using stored procedure. (ELMAH for Asp.Net Core)

An Extension of ILogger to log Data into MsSql DB, This is an alternative for ELMAH in asp.net mvc.  currently there is no page to view errors, or error details inside, but anyone can help will be appreciated.

The Default LogLevel for this Extension is Error, (if not specified).

# How to Install.

1- Before using this tool, Create a Database, and add Connection string into appsetting.json like below.

```json
{
  "ConnectionStrings": {
    "LoggerConnection" : "Server=(localdb)\\MSSQLLocalDB;Database=Logs;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning"
    }
  }
}
```
2- Execute CreateScript.sql file in database

## Update Script:
**Not:** If you are updating from version 2.0.1 or below, please run update script below:
```sql
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Logs
	DROP CONSTRAINT DF_Logs_LogId
GO
CREATE TABLE dbo.Tmp_Logs
	(
	TimeUtc datetime NOT NULL,
	LogId uniqueidentifier NOT NULL,
	RequestId nvarchar(60) NULL,
	Application nvarchar(100) NULL,
	Category nvarchar(60) NOT NULL,
	Type nvarchar(100) NOT NULL,
	Source nvarchar(60) NOT NULL,
	FileName nvarchar(400) NOT NULL,
	Message nvarchar(500) NOT NULL,
	[User] nvarchar(50) NOT NULL,
	StatusCode int NOT NULL,
	StackTrace nvarchar(4000) NOT NULL,
	ExceptionDetail ntext NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Logs SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_Logs ADD CONSTRAINT
	DF_Logs_LogId DEFAULT (newid()) FOR LogId
GO
IF EXISTS(SELECT * FROM dbo.Logs)
	 EXEC('INSERT INTO dbo.Tmp_Logs (TimeUtc, LogId, Application, Category, Type, Source, FileName, Message, [User], StatusCode, StackTrace, ExceptionDetail)
		SELECT TimeUtc, LogId, Application, Category, Type, Source, FileName, Message, [User], StatusCode, StackTrace, ExceptionDetail FROM dbo.Logs WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.Logs
GO
EXECUTE sp_rename N'dbo.Tmp_Logs', N'Logs', 'OBJECT' 
GO
ALTER TABLE dbo.Logs ADD CONSTRAINT
	PK_Log_ID PRIMARY KEY NONCLUSTERED 
	(
	LogId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT

Go
ALTER PROCEDURE [dbo].[spInsertLog]
(
    @TimeUtc DATETIME,
    @LogId UNIQUEIDENTIFIER,
    @RequestId NVARCHAR(60) = null,
    @Application NVARCHAR(100) = null,
    @Category NVARCHAR(60),
    @Type NVARCHAR(100),
    @Source NVARCHAR(60),
    @FileName NVARCHAR(400),
    @Message NVARCHAR(500),
    @User NVARCHAR(50),
    @ExceptionDetail NTEXT,
    @StatusCode INT,
    @StackTrace NVARCHAR(4000)
)
AS

    SET NOCOUNT ON

    INSERT
    INTO
        [dbo].[Logs]
        (
            [TimeUtc],
            [LogId],
            [RequestId],
            [Application],
            [Category],            
            [Type],
            [Source],
            [FileName],
            [Message],
            [User],
            [ExceptionDetail],
            [StatusCode],
            [StackTrace]
        )
    VALUES
        (
            @TimeUtc,
            @LogId,
            @RequestId,
            @Application,
            @Category,            
            @Type,
            @Source,
            @FileName,
            @Message,
            @User,
            @ExceptionDetail,
            @StatusCode,
            @StackTrace
        )

```
## Create Script
**Not** if this is first time you are installing this nuget package, run below script to create tables and sp.

```sql
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Logs](
	[TimeUtc] [datetime] NOT NULL,	
	[LogId] [uniqueidentifier] NOT NULL,
    [RequestId] [navrchar](60) NULL,
	[Application] [nvarchar](100) NULL,
	[Category] [nvarchar](60) NOT NULL,
	[Type] [nvarchar](100) NOT NULL,
	[Source] [nvarchar](60) NOT NULL,
	[FileName] [nvarchar] (400) NOT NULL,
	[Message] [nvarchar](500) NOT NULL,
	[User] [nvarchar](50) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[StackTrace] [nvarchar] (4000) NOT NULL,
	[ExceptionDetail] [ntext] NOT NULL,
 CONSTRAINT [PK_Log_ID] PRIMARY KEY NONCLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  StoredProcedure [dbo].[spInsertLog]  ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spInsertLog]
(
    @TimeUtc DATETIME,
    @LogId UNIQUEIDENTIFIER,
    @RequestId NVARCHAR(60) = null,
    @Application NVARCHAR(100) = null,
    @Category NVARCHAR(60),
    @Type NVARCHAR(100),
    @Source NVARCHAR(60),
    @FileName NVARCHAR(400),
    @Message NVARCHAR(500),
    @User NVARCHAR(50),
    @ExceptionDetail NTEXT,
    @StatusCode INT,
    @StackTrace NVARCHAR(4000)
)
AS

    SET NOCOUNT ON

    INSERT
    INTO
        [dbo].[Logs]
        (
            [TimeUtc],
            [LogId],
            [RequestId],
            [Application],
            [Category],            
            [Type],
            [Source],
            [FileName],
            [Message],
            [User],
            [ExceptionDetail],
            [StatusCode],
            [StackTrace]
        )
    VALUES
        (
            @TimeUtc,
            @LogId,
            @RequestId,
            @Application,
            @Category,            
            @Type,
            @Source,
            @FileName,
            @Message,
            @User,
            @ExceptionDetail,
            @StatusCode,
            @StackTrace
        )
GO

/****** Object:  Default [DF_Logs_LogId] ******/
ALTER TABLE [dbo].[Logs] ADD  CONSTRAINT [DF_Logs_LogId]  DEFAULT (newid()) FOR [LogId]
GO
```
 

3- in Asp.net Core Web Application in startup.cs add following codes:

3-1- 
```csharp
public void ConfigureServices(IServiceCollection services)
{
  // Add framework services.

  // Add This Line To access HttpContext from within the MsSqlLogger to get the user name, trace Identifier that gets error also for returning back logId inside httpContext.
  services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
  services.AddMvc();
}
```

3-2-
```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IHttpContextAccessor httpContextAccessor)
{
    loggerFactory.AddConsole(Configuration.GetSection("Logging"));
    loggerFactory.AddDebug();

    // Add This To Log to MsSql Db
    loggerFactory.AddMsSql(Configuration.GetConnectionString("LoggerConnection"), httpContextAccessor, "SampleApplication");

	// Also you can add as Provider as below:
	//loggerFactory.AddProvider(new MsSqlLoggerProvider((_, LogLevel) => LogLevel >= LogLevel.Trace,
        //     Configuration.GetConnectionString("LoggerConnection"), null,"SampleApplication"));            

    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseBrowserLink();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
    }

    app.UseStaticFiles();

    app.UseMvc(routes =>
    {
        routes.MapRoute(
            name: "default",
            template: "{controller=Home}/{action=Index}/{id?}");
    });
}
```
     
# Using in Asp.net Core Web Application Targetting .NetFramework

To Use in Asp.Net Web Application targetting .NetFramework you should also add following nuget packages manually to your project.

- **System.Security.Claims (4.3)** from 
<a href='https://www.nuget.org/packages/System.Security.Claims/'>NUGET</a>

- **System.Diagnostics.StackTrace (4.3)** from 
<a href='https://www.nuget.org/packages/System.Diagnostics.StackTrace'>NUGET</a>





