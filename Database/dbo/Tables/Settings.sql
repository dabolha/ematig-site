﻿CREATE TABLE [dbo].[Settings](
	[Key] [varchar](100) NOT NULL,
	[Value] [varchar](1000) NOT NULL,
	[Name] [varchar](200) NOT NULL
 CONSTRAINT [PK_Settings] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO