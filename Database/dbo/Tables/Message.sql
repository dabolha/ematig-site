CREATE TABLE [dbo].[Message]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [FromEmail] NVARCHAR(128) NOT NULL,  
    [ToEmail] NVARCHAR(128) NULL, 
    [Subject] NVARCHAR(128) NOT NULL, 
    [Body] NVARCHAR(MAX) NOT NULL, 
	[MessageTypeId] SMALLINT NOT NULL, 
    [CreationDate] DATETIME NOT NULL, 
    [SentDate] DATETIME NULL DEFAULT GetDate(), 
    CONSTRAINT [FK_Message_MessageType] FOREIGN KEY ([MessageTypeId]) REFERENCES [MessageType]([Id])
)
