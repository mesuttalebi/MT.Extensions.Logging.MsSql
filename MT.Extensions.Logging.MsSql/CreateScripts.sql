﻿SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Logs](
	[TimeUtc] [datetime] NOT NULL,	
	[LogId] [uniqueidentifier] NOT NULL,
    [RequestId] [nvarchar](60) NULL,
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