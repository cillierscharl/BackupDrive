
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 09/08/2013 22:37:06
-- Generated from EDMX file: C:\Users\charl.cilliers\Documents\Visual Studio 2012\Projects\Backup\Backup.DataAccess\ApertureBackupModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [ApertureBackup];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_AccountTypeUserAccount]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserAccounts] DROP CONSTRAINT [FK_AccountTypeUserAccount];
GO
IF OBJECT_ID(N'[dbo].[FK_BackupFileStateBackupFile]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BackupFiles] DROP CONSTRAINT [FK_BackupFileStateBackupFile];
GO
IF OBJECT_ID(N'[dbo].[FK_BackupFolderBackupFile]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BackupFiles] DROP CONSTRAINT [FK_BackupFolderBackupFile];
GO
IF OBJECT_ID(N'[dbo].[FK_UserAccountBackupFolder]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BackupFolders] DROP CONSTRAINT [FK_UserAccountBackupFolder];
GO
IF OBJECT_ID(N'[dbo].[FK_UserUserAccount]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserAccounts] DROP CONSTRAINT [FK_UserUserAccount];
GO
IF OBJECT_ID(N'[dbo].[FK_UserUserLog]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserLogs] DROP CONSTRAINT [FK_UserUserLog];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AccountTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AccountTypes];
GO
IF OBJECT_ID(N'[dbo].[BackupFiles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BackupFiles];
GO
IF OBJECT_ID(N'[dbo].[BackupFileStates]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BackupFileStates];
GO
IF OBJECT_ID(N'[dbo].[BackupFolders]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BackupFolders];
GO
IF OBJECT_ID(N'[dbo].[UserAccounts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserAccounts];
GO
IF OBJECT_ID(N'[dbo].[UserLogs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserLogs];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'BackupFiles'
CREATE TABLE [dbo].[BackupFiles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [BackupFolderId] int  NOT NULL,
    [FileName] nvarchar(max)  NOT NULL,
    [FileSize] bigint  NOT NULL,
    [BackupFileStateId] int  NOT NULL
);
GO

-- Creating table 'BackupFolders'
CREATE TABLE [dbo].[BackupFolders] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [AbsoluteFolderPath] nvarchar(max)  NOT NULL,
    [UserAccountId] int  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Username] nvarchar(max)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [Enabled] bit  NOT NULL,
    [Email] nvarchar(max)  NULL,
    [UserTypeId] int  NOT NULL
);
GO

-- Creating table 'UserAccounts'
CREATE TABLE [dbo].[UserAccounts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int  NOT NULL,
    [AccountUniqueId] nvarchar(max)  NOT NULL,
    [AccountDataQuota] bigint  NULL
);
GO

-- Creating table 'UserTypes'
CREATE TABLE [dbo].[UserTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Type] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'UserLogs'
CREATE TABLE [dbo].[UserLogs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int  NOT NULL
);
GO

-- Creating table 'BackupFileStates'
CREATE TABLE [dbo].[BackupFileStates] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [State] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'BackupFiles'
ALTER TABLE [dbo].[BackupFiles]
ADD CONSTRAINT [PK_BackupFiles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BackupFolders'
ALTER TABLE [dbo].[BackupFolders]
ADD CONSTRAINT [PK_BackupFolders]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserAccounts'
ALTER TABLE [dbo].[UserAccounts]
ADD CONSTRAINT [PK_UserAccounts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserTypes'
ALTER TABLE [dbo].[UserTypes]
ADD CONSTRAINT [PK_UserTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserLogs'
ALTER TABLE [dbo].[UserLogs]
ADD CONSTRAINT [PK_UserLogs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BackupFileStates'
ALTER TABLE [dbo].[BackupFileStates]
ADD CONSTRAINT [PK_BackupFileStates]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [BackupFolderId] in table 'BackupFiles'
ALTER TABLE [dbo].[BackupFiles]
ADD CONSTRAINT [FK_BackupFolderBackupFile]
    FOREIGN KEY ([BackupFolderId])
    REFERENCES [dbo].[BackupFolders]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BackupFolderBackupFile'
CREATE INDEX [IX_FK_BackupFolderBackupFile]
ON [dbo].[BackupFiles]
    ([BackupFolderId]);
GO

-- Creating foreign key on [UserId] in table 'UserAccounts'
ALTER TABLE [dbo].[UserAccounts]
ADD CONSTRAINT [FK_UserUserAccount]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserUserAccount'
CREATE INDEX [IX_FK_UserUserAccount]
ON [dbo].[UserAccounts]
    ([UserId]);
GO

-- Creating foreign key on [UserAccountId] in table 'BackupFolders'
ALTER TABLE [dbo].[BackupFolders]
ADD CONSTRAINT [FK_UserAccountBackupFolder]
    FOREIGN KEY ([UserAccountId])
    REFERENCES [dbo].[UserAccounts]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserAccountBackupFolder'
CREATE INDEX [IX_FK_UserAccountBackupFolder]
ON [dbo].[BackupFolders]
    ([UserAccountId]);
GO

-- Creating foreign key on [UserId] in table 'UserLogs'
ALTER TABLE [dbo].[UserLogs]
ADD CONSTRAINT [FK_UserUserLog]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserUserLog'
CREATE INDEX [IX_FK_UserUserLog]
ON [dbo].[UserLogs]
    ([UserId]);
GO

-- Creating foreign key on [BackupFileStateId] in table 'BackupFiles'
ALTER TABLE [dbo].[BackupFiles]
ADD CONSTRAINT [FK_BackupFileStateBackupFile]
    FOREIGN KEY ([BackupFileStateId])
    REFERENCES [dbo].[BackupFileStates]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BackupFileStateBackupFile'
CREATE INDEX [IX_FK_BackupFileStateBackupFile]
ON [dbo].[BackupFiles]
    ([BackupFileStateId]);
GO

-- Creating foreign key on [UserTypeId] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_UserTypeUser]
    FOREIGN KEY ([UserTypeId])
    REFERENCES [dbo].[UserTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserTypeUser'
CREATE INDEX [IX_FK_UserTypeUser]
ON [dbo].[Users]
    ([UserTypeId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------