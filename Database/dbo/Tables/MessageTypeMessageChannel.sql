CREATE TABLE [dbo].[MessageTypeMessageChannel]
(
	[MessageTypeId] SMALLINT NOT NULL, 
	[MessageChannelId] SMALLINT NOT NULL,  
	[Subject] NVARCHAR(128) NOT NULL, 
    [Body] NVARCHAR(MAX) NOT NULL, 
    CONSTRAINT [FK_MessageTypeMessageChannel_MessageType] FOREIGN KEY (MessageTypeId) REFERENCES [MessageType]([Id]), 
    CONSTRAINT [FK_MessageTypeMessageChannel_MessageChannel] FOREIGN KEY (MessageChannelId) REFERENCES [MessageChannel]([Id]), 
    CONSTRAINT [PK_MessageTypeMessageChannel] PRIMARY KEY ([MessageTypeId], [MessageChannelId]), 
)
