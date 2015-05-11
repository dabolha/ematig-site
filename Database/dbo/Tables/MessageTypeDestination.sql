CREATE TABLE [dbo].[MessageTypeDestination]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[MessageTypeId] SMALLINT NOT NULL, 
	[ToEmail] NVARCHAR(128) NOT NULL, 
	[Subject] NVARCHAR(128) NOT NULL, 
    [Body] NVARCHAR(MAX) NOT NULL, 
	CONSTRAINT [FK_MessageTypeDestination_MessageType] FOREIGN KEY (MessageTypeId) REFERENCES [MessageType]([Id])
)
