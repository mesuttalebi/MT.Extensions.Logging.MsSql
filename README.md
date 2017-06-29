# MT.Extensions.Logging.MsSql
An .net Core Logger Extension that logs to MsSql server using stored procedure. (ELMAH for Asp.Net Core)

An Extension of ILogger to log Data into MsSql DB, This is an alternative for ELMAH in asp.net mvc. but for now error detail pages not working.

The Default LogLevel for this Extension is Error, (if not specified).

# How to Install.

1- Before using this tool, Create a Database, and add Connection string into appsetting.json like below.

<pre>{
  "ConnectionStrings": {
    "LoggerConnection" : "Server=(localdb)\\MSSQLLocalDB;Database=Logs;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning"
    }
  }
}</pre>

2- Execute CreateScript.sql file in database

<pre>
ET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Logs](
	[LogId] [uniqueidentifier] NOT NULL,
	[Category] [nvarchar](60) NOT NULL,
	[Type] [nvarchar](100) NOT NULL,
	[Source] [nvarchar](60) NOT NULL,
	[Message] [nvarchar](500) NOT NULL,
	[User] [nvarchar](50) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[TimeUtc] [datetime] NOT NULL,
	[Sequence] [int] IDENTITY(1,1) NOT NULL,
	[ExceptionDetail] [ntext] NOT NULL,
 CONSTRAINT [PK_Log_ID] PRIMARY KEY NONCLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Log_Id_App_Time_Seq] ON [dbo].[Logs] 
(
	[Category] ASC,
	[TimeUtc] DESC,
	[Sequence] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
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
    @Message NVARCHAR(500),
    @User NVARCHAR(50),
    @ExceptionDetail NTEXT,
    @StatusCode INT,
    @TimeUtc DATETIME
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
            [Message],
            [User],
            [ExceptionDetail],
            [StatusCode],
            [TimeUtc]
        )
    VALUES
        (
            @LogId,
            @Category,            
            @Type,
            @Source,
            @Message,
            @User,
            @ExceptionDetail,
            @StatusCode,
            @TimeUtc
        )
GO

/****** Object:  Default [DF_Logs_LogId] ******/
ALTER TABLE [dbo].[Logs] ADD  CONSTRAINT [DF_Logs_LogId]  DEFAULT (newid()) FOR [LogId]
GO
</pre>


3- in Asp.net Core Web Application in startup.cs add following codes:
3-1- 
<pre>
public void ConfigureServices(IServiceCollection services)
{
  // Add framework services.

  // Add This Line To access HttpContext from within the MsSqlLogger
  services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
  services.AddMvc();
}
</pre>

3-2-
<pre>
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
</pre>
     










