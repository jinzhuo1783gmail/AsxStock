CREATE TABLE [dbo].[ShortHistories]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
    [Symbol] VARCHAR(20) NOT NULL, 
	[CompanyName] VARCHAR(max)  NULL,
	[ProductClass] VARCHAR(20) NOT NULL, 
    [TotalIssued] BigINT NULL DEFAULT 0,
    [TotalShort] bigINT NULL DEFAULT 0,
    [Percentage] float NOT NULL, 
    [ShortDate] DATETIME Not NULL, 
	[UploadDate] DATETIME Not NULL, 
)

CREATE INDEX idx_ShortHistories_Symbol_ShortDate ON ShortHistories (Symbol, ShortDate);
CREATE INDEX idx_ShortHistories_Symbol ON ShortHistories (Symbol);


--drop table [dbo].[Companies]

CREATE TABLE [dbo].[Companies]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
    [Symbol] VARCHAR(20) NOT NULL, 
	[CompanyName] VARCHAR(max)  NULL,
	[Catergory] VARCHAR(200) null,
	[SubCatergory] VARCHAR(200) null,
	[SectorName] VARCHAR(200) null,
	[TotalShares] bigint  DEFAULT 0,
	[LastPrice] float not null Default 0,
	[Analysis] BIT  NOT NULL DEFAULT 0,
	[UploadDate] DATETIME Not NULL, 
)

CREATE INDEX idx_Companys_Symbol ON [Companies] (Symbol);


--drop table [dbo].[Announcements]

CREATE TABLE [dbo].[Announcements]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
	[AsxDocumentId] BIGINT NOT NULL,
    [Symbol] VARCHAR(20) NOT NULL, 
	[FileURL] VARCHAR(200) NOT NULL, 
	[FileContent] varbinary(max) NOT NULL, 
	[FileText] VARCHAR(max) NOT NULL, 
	[FileSummary1] VARCHAR(max) NOT NULL,
	[FileSummary2] VARCHAR(max) NOT NULL,
	[FileSummary3] VARCHAR(max) NOT NULL,
	[ReleaseDate]  DATETIME Not NULL, 
	[UploadDate] DATETIME Not NULL, 
)

CREATE INDEX idx_Announcement_AsxDocumentId ON [Announcements] (AsxDocumentId);

CREATE INDEX idx_Announcement_Symbol ON [Announcements] (Symbol);

CREATE INDEX idx_Announcement_Symbol_AsxDocumentId ON [Announcements] (Symbol, AsxDocumentId);


--drop table [dbo].[Prices]
CREATE TABLE [dbo].[Prices]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
    [Symbol] VARCHAR(20) NOT NULL, 
	[Date] DATETIME NULL,
	[Open] float NULL, 
	[High] float NULL, 
	[Low] float NULL, 
	[Close] float NULL,
	[Volumn] float NULL,
	[CloseAdj] float NULL,
	[UploadDate] DATETIME Not NULL, 
)

CREATE INDEX idx_Prices_Symbol ON [Prices] (Symbol);

CREATE INDEX idx_Prices_Symbol_Date ON [Prices] (Symbol, Date);

CREATE TABLE [dbo].[ScheduleSettings]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
    [TaskName] VARCHAR(20) NOT NULL, 
	[ScheduleTime] TIME NOT NULL,
	[IsActive] bit NULL, 
	[UploadDate] DATETIME Not NULL, 
)


CREATE TABLE [dbo].[ScheduleTaskHistories]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
    [TaskName] VARCHAR(20) NOT NULL, 
	[ProcessDateTime] DATETIME NOT NULL,
	[LogHistory] varbinary(max) NOT NULL,
	[UploadDate] DATETIME Not NULL, 
)

CREATE INDEX idx_ScheduleTaskHistories_TaskName ON [ScheduleTaskHistories] (TaskName);

Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('CompanyInformation', '08:00:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('ShortList', '12:00:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('ShortList', '15:00:00', 1, '2024-05-05T08:00:00')

Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('CheckCompany', '13:00:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('CheckCompany', '16:00:00', 1, '2024-05-05T08:00:00')

Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('Annoucement', '11:00:00', 1, '2024-05-05T08:00:00')

Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('PatchCompany', '11:00:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('PatchCompany', '13:00:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('PatchCompany', '15:00:00', 1, '2024-05-05T08:00:00')

Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('HistoricalPrice', '09:00:00', 1, '2024-05-05T08:00:00')

Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '10:15:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '10:30:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '10:45:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '11:00:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '11:15:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '11:30:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '11:45:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '12:00:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '12:15:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '12:30:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '12:45:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '13:00:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '13:15:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '13:30:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '13:45:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '14:00:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '14:15:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '14:30:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '14:45:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '15:00:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '15:15:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '15:30:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '15:45:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '16:00:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '16:15:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '16:30:00', 1, '2024-05-05T08:00:00')
Insert into [dbo].[ScheduleSettings] ([TaskName], [ScheduleTime], [IsActive], [UploadDate]) values ('GetQuote', '17:15:00', 1, '2024-05-05T08:00:00')


select * from [dbo].[ScheduleSettings]


CREATE TABLE [dbo].[SectorIndustryInvestmentFlowInOut]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
    [Catergory] VARCHAR(200) null,
	[SubCatergory] VARCHAR(200) not null,
	[SectorName] VARCHAR(200) not null,
	[InvestmentInOut] float not null,
	[UploadDate] DATETIME Not NULL, 
)

CREATE INDEX idx_SectorIndustryInvestmentFlowInOut_Category_Sector ON [SectorIndustryInvestmentFlowInOut] ([SectorName], [Catergory] );
CREATE INDEX idx_SectorIndustryInvestmentFlowInOut_Category ON [SectorIndustryInvestmentFlowInOut] ([Catergory] );

CREATE TABLE [dbo].[IndustrySubCatInvestmentsFlowInOut]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
    [Catergory] VARCHAR(200) null,
	[SubCatergory] VARCHAR(200) not null,
	[SectorName] VARCHAR(200) not null,
	[InvestmentInOut] float not null,
	[UploadDate] DATETIME Not NULL, 
)

CREATE INDEX idx_IndustrySubCatInvestmentFlowInOut_Catergory_Sector ON [IndustrySubCatInvestmentsFlowInOut] ( [Catergory], [SubCatergory]);
CREATE INDEX idx_IndustrySubCatInvestmentFlowInOut_Catergory ON [IndustrySubCatInvestmentsFlowInOut] ([Catergory] );