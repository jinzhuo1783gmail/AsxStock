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


CREATE TABLE [dbo].[SubCategorySummaryQuestions]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
    [SubCategory] VARCHAR(20) NOT NULL, 
	[Sequence] INT NOT NULL,
	[Question] NVARCHAR(max) NOT NULL,
	[IsActive] bit NULL, 
	[UploadDate] DATETIME Not NULL, 
)

CREATE INDEX idx_SubCategorySummaryQuestions_SubCategory ON [SubCategorySummaryQuestions] (SubCategory);