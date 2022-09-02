DROP TABLE [dbo].[tblAdmins];
GO

CREATE TABLE [dbo].[tblAdmins] (
	[admin_id] int IDENTITY(1,1),
	CONSTRAINT PK_User PRIMARY KEY (admin_id)
);
GO