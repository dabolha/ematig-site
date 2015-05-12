

if not exists(select [Id] from [dbo].[MessageChannel] Where [Id] like 1)
begin	
	INSERT     [dbo].[MessageChannel] ([Id], [Name]) VALUES (1, N'Email')
end

