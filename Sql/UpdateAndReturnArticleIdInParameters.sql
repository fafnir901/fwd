CREATE FUNCTION [dbo].[UpdateAndReturnArticleIdInParameters]
(
	@PrevParameters nvarchar(max),
	@ArticleID int
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	declare @table dbo.Hierarchy
	declare @result nvarchar(max)
	
	INSERT INTO @table
	SELECT * FROM parseJSON(@PrevParameters)
	
	UPDATE @table
	SET StringValue = @ArticleID
	WHERE Name = 'ArticleId'
	
	SELECT @result = dbo.ToJson(@table)
	return @result
END
GO


