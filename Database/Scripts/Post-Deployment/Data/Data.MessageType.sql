

if not exists(select [Id] from [dbo].[MessageType] Where [Id] like 1)
begin	
	INSERT     [dbo].[MessageType] ([Id], [Name]) VALUES (1, N'ContactRequest')
end

if not exists(select [Id] from [dbo].[MessageType] Where [Id] like 2)
begin	
	INSERT     [dbo].[MessageType] ([Id], [Name]) VALUES (2, N'Custom')
end