CREATE TABLE [dbo].[MessageTypeDestination]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[MessageTypeId] SMALLINT NOT NULL, 
	[ToEmail] NVARCHAR(128) NOT NULL, 
	[Subject] NVARCHAR(128) NULL, 
    [Body] NVARCHAR(MAX) NULL, 
	CONSTRAINT [FK_MessageTypeDestination_MessageType] FOREIGN KEY (MessageTypeId) REFERENCES [MessageType]([Id])
)
