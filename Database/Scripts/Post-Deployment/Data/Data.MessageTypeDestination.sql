

if not exists(select [MessageTypeId] from [dbo].[MessageTypeDestination] Where [MessageTypeId] like 1)
begin	
	INSERT     [dbo].[MessageTypeDestination] ([MessageTypeId] ,[ToEmail] ,[Subject] ,[Body]) VALUES (1,  N'bollysystem@gmail.com', N'Pedido de contacto.', null)
end