CREATE TABLE [dbo].[User] (
    [Id]                   BIGINT NOT NULL,
	[AuthId]               NVARCHAR (128) NOT NULL,
    [Email]                NVARCHAR (256) NULL,
    [PhoneNumber]          NVARCHAR (MAX) NULL,
    [FirstName]            NVARCHAR (MAX) NULL,
    [LastName]             NVARCHAR (MAX) NULL,
    [Gender]               NVARCHAR (MAX) NULL,
    [Address]              NVARCHAR (MAX) NULL,
    [PostCode]             NVARCHAR (MAX) NULL,
    [MobilePhoneNumber]    NVARCHAR (MAX) NULL,
    [BirthDate]            DATETIME       DEFAULT GetDate() NOT NULL,
    [CreationDate]         DATETIME       DEFAULT GetDate() NOT NULL,
    [ModificationDate]     DATETIME       DEFAULT GetDate() NOT NULL,
    CONSTRAINT [PK_dbo.User] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_User_ToTable] FOREIGN KEY ([AuthId]) REFERENCES [AspNetUsers]([Id])
);



