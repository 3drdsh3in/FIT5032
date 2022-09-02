DROP TABLE [dbo].[tblUsers];
GO

CREATE TABLE [dbo].[tblUsers] (
	[user_id] int IDENTITY(1,1),
	[user_fname] VARCHAR(100),
	[user_lname] VARCHAR(100),
	[is_staff] [bit] NOT NULL,
	CONSTRAINT PK_User PRIMARY KEY ([user_id])
);
GO