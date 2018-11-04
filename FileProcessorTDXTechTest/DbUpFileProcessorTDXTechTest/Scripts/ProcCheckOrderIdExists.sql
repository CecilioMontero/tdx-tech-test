---------------------------------------------------------------------------------------------------------------------------------------------------------
--Proc CheckOrderIdExists
---------------------------------------------------------------------------------------------------------------------------------------------------------

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CheckOrderIdExists]'))
BEGIN
	PRINT 'Create PROC [dbo].[CheckOrderIdExists]'

END
GO

CREATE PROCEDURE [dbo].[CheckOrderIdExists]

AS
DECLARE	@ListIds table (Ids uniqueidentifier)


BEGIN
	SELECT CAST(CASE WHEN EXISTS 
	(SELECT 1 FROM Orders os 
	JOIN @ListIds li on li.Ids = os.orderid
	where os.orderId = li.Ids)then 1 else 0 end as bit)

	
END
GO