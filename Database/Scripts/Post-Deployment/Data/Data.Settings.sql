

if not exists(select [Key] from [dbo].[Settings] Where [Key] like 'EmailFrom')
begin	
	INSERT     [dbo].[Settings] ([Key], [Value], [Name]) VALUES (N'EmailFrom', N'adilsonpaula@gmail.com', N'Email - From.')
end