CREATE TABLE [dbo].[UserStats](
	[UserId] [nvarchar](128) NOT NULL,
	[Victories] [real] NOT NULL,
	[Defeats] [real] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)
)
GO

CREATE INDEX IX_UserStats_Victories ON UserStats( [Victories] );
CREATE INDEX IX_UserStats_Defeats ON UserStats( [Defeats] );
GO

CREATE PROCEDURE GenerateBoard
	@boardName AS nvarchar(128), 
	@count AS int,
	@focusUserId AS nvarchar(128)
AS
BEGIN
	SET NOCOUNT ON

	IF ((@focusUserId IS NULL) OR  (@focusUserId = ''))
	BEGIN
		IF @boardName = 'Victories'
			SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY Victories          DESC) AS Id, * FROM UserStats) T WHERE T.Id <= @count ORDER BY T.Id
		ELSE IF @boardName = 'Defeats'
			SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY Defeats           ASC) AS Id, * FROM UserStats) T WHERE T.Id <= @count ORDER BY T.Id
	END
	ELSE
	BEGIN
		DECLARE @userPos AS int
		DECLARE @topPos AS int
		DECLARE @bottomPos AS int
	    
		IF @boardName = 'Victories'
		BEGIN
			SELECT @userPos = T.Id FROM (SELECT ROW_NUMBER() OVER(ORDER BY Victories DESC) AS Id, UserId FROM UserStats) T WHERE T.UserId = @focusUserId ORDER BY T.Id
			SET @topPos = @userPos - Floor(@count/2)
			SET @bottomPos = @userPos + Floor(@count/2)
			
			SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY Victories DESC) AS Id, * FROM UserStats) T WHERE T.Id BETWEEN @topPos AND @bottomPos ORDER BY T.Id
		END
		ELSE IF @boardName = 'Defeats'
		BEGIN
			SELECT @userPos = T.Id FROM (SELECT ROW_NUMBER() OVER(ORDER BY Defeats ASC) AS Id, UserId FROM UserStats) T WHERE T.UserId = @focusUserId ORDER BY T.Id
			SET @topPos = @userPos - Floor(@count/2)
			SET @bottomPos = @userPos + Floor(@count/2)
			
			SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY Defeats ASC) AS Id, * FROM UserStats) T WHERE T.Id BETWEEN @topPos AND @bottomPos ORDER BY T.Id
		END
	END
END;
GO