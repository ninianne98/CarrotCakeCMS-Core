CREATE TABLE [dbo].[AspNetCache] (
    [Id]                         NVARCHAR (512)     COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL,
    [Value]                      VARBINARY (MAX)    NOT NULL,
    [ExpiresAtTime]              DATETIMEOFFSET (7) NOT NULL,
    [SlidingExpirationInSeconds] BIGINT             NULL,
    [AbsoluteExpiration]         DATETIMEOFFSET (7) NULL,
    CONSTRAINT [PK_AspNetCache] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Index_ExpiresAtTime]
    ON [dbo].[AspNetCache]([ExpiresAtTime] ASC);

