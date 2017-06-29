SET ANSI_NULLS ON
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
/****** Object:  StoredProcedure [dbo].[spGetErrorXml]   ***/
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
--CREATE PROCEDURE [dbo].[spGetErrorXml]
--(
--    @Category NVARCHAR(60),
--    @LogId UNIQUEIDENTIFIER
--)
--AS

--    SET NOCOUNT ON

--    SELECT 
--        [ExceptionDetail]
--    FROM 
--        [dbo].[Logs]
--    WHERE
--        [LogId] = @LogId
--    AND
--        [Category] = @Category
--GO

/****** Object:  StoredProcedure [dbo].[spGetErrorsXml]  ******/
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
--CREATE PROCEDURE [dbo].[spGetErrorsXml]
--(
--    @Category NVARCHAR(60),
--    @PageIndex INT = 0,
--    @PageSize INT = 15,
--    @TotalCount INT OUTPUT
--)
--AS 

--    SET NOCOUNT ON

--    DECLARE @FirstTimeUTC DATETIME
--    DECLARE @FirstSequence INT
--    DECLARE @StartRow INT
--    DECLARE @StartRowIndex INT

--    SELECT 
--        @TotalCount = COUNT(1) 
--    FROM 
--        [Logs]
--    WHERE 
--        [Category] = @Category

--    -- Get the ID of the first error for the requested page

--    SET @StartRowIndex = @PageIndex * @PageSize + 1

--    IF @StartRowIndex <= @TotalCount
--    BEGIN

--        SET ROWCOUNT @StartRowIndex

--        SELECT  
--            @FirstTimeUTC = [TimeUtc],
--            @FirstSequence = [Sequence]
--        FROM 
--            [Logs]
--        WHERE   
--            [Category] = @Category
--        ORDER BY 
--            [TimeUtc] DESC, 
--            [Sequence] DESC

--    END
--    ELSE
--    BEGIN

--        SET @PageSize = 0

--    END

--    -- Now set the row count to the requested page size and get
--    -- all records below it for the pertaining application.

--    SET ROWCOUNT @PageSize

--    SELECT 
--        LogId       = [LogId], 
--        application = [Category],        
--        type        = [Type],
--        source      = [Source],
--        message     = [Message],
--        [user]      = [User],
--        statusCode  = [StatusCode], 
--        time        = CONVERT(VARCHAR(50), [TimeUtc], 126) + 'Z'
--    FROM 
--        [Logs] log
--    WHERE
--        [Category] = @Category
--    AND
--        [TimeUtc] <= @FirstTimeUTC
--    AND 
--        [Sequence] <= @FirstSequence
--    ORDER BY
--        [TimeUtc] DESC, 
--        [Sequence] DESC
--    FOR
--        XML AUTO
--GO
/****** Object:  Default [DF_Logs_LogId] ******/
ALTER TABLE [dbo].[Logs] ADD  CONSTRAINT [DF_Logs_LogId]  DEFAULT (newid()) FOR [LogId]
GO