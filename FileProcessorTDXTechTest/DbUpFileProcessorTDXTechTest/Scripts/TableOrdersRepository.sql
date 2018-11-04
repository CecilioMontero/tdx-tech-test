IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Orders]'))
BEGIN
	PRINT 'Create table [dbo].[Orders]'

	CREATE TABLE [dbo].[Orders](
	OrderId uniqueidentifier not null rowguidcol primary key,
	Country NVARCHAR(100) NOT NULL,
	ItemType NVARCHAR(100) NOT NULL,
	OrderDate DateTime NOT NULL,
	UnitsSold int NOT NULL,
	UnitPrice float NOT NULL,
	);

END
GO