# MT.Extensions.Logging.MsSql
An .net Core Logger Extension that logs to MsSql server using stored procedure. (ELMAH for Asp.Net Core)

An Extension of ILogger to log Data into MsSql DB, This is an alternative for ELMAH in asp.net mvc. but for now error detail pages not working.

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

```sql
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Logs](
	[LogId] [uniqueidentifier] NOT NULL,
	[Category] [nvarchar](60) NOT NULL,
	[Type] [nvarchar](100) NOT NULL,
	[Source] [nvarchar](60) NOT NULL,
	[FileName] [nvarchar] (400) NOT NULL,
	[Message] [nvarchar](500) NOT NULL,
	[User] [nvarchar](50) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[TimeUtc] [datetime] NOT NULL,
	[Sequence] [int] IDENTITY(1,1) NOT NULL,
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
    @LogId UNIQUEIDENTIFIER,
    @Category NVARCHAR(60),
    @Type NVARCHAR(100),
    @Source NVARCHAR(60),
	@FileName NVARCHAR(400),
    @Message NVARCHAR(500),
    @User NVARCHAR(50),
    @ExceptionDetail NTEXT,
    @StatusCode INT,
    @TimeUtc DATETIME,
	@StackTrace NVARCHAR(4000)
)
AS

    SET NOCOUNT ON

    INSERT
    INTO
        [dbo].[Logs]
        (
            [LogId],
            [Category],            
            [Type],
            [Source],
	    [FileName],
            [Message],
            [User],
            [ExceptionDetail],
            [StatusCode],
            [TimeUtc],
	    [StackTrace]
        )
    VALUES
        (
            @LogId,
            @Category,            
            @Type,
            @Source,
	    @FileName,
            @Message,
            @User,
            @ExceptionDetail,
            @StatusCode,
            @TimeUtc,
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

  // Add This Line To access HttpContext from within the MsSqlLogger
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
            loggerFactory.AddMsSql(Configuration.GetConnectionString("LoggerConnection"), httpContextAccessor);

            
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





