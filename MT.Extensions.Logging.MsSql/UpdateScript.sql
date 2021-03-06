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
