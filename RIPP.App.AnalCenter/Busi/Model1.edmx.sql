
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 08/14/2012 01:12:35
-- Generated from EDMX file: D:\3506\03chemometrics\RIPP_DEMO\src\RIPP.App.AnalCenter\Busi\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [NIRCenter];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AllMethod]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AllMethod];
GO
IF OBJECT_ID(N'[dbo].[OilData]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OilData];
GO
IF OBJECT_ID(N'[dbo].[Properties]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Properties];
GO
IF OBJECT_ID(N'[dbo].[S_Moudle]', 'U') IS NOT NULL
    DROP TABLE [dbo].[S_Moudle];
GO
IF OBJECT_ID(N'[dbo].[S_User]', 'U') IS NOT NULL
    DROP TABLE [dbo].[S_User];
GO
IF OBJECT_ID(N'[dbo].[Specs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Specs];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'S_Moudle'
CREATE TABLE [dbo].[S_Moudle] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [text] varchar(50)  NULL,
    [pID] int  NOT NULL,
    [name] varchar(50)  NOT NULL,
    [role1] bit  NOT NULL,
    [role2] bit  NOT NULL
);
GO

-- Creating table 'AllMethod'
CREATE TABLE [dbo].[AllMethod] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [AddTime] datetime  NOT NULL,
    [FullPath] varchar(800)  NOT NULL,
    [UserID] int  NOT NULL,
    [MD5Str] varchar(200)  NOT NULL,
    [Contents] varbinary(max)  NOT NULL
);
GO

-- Creating table 'S_User'
CREATE TABLE [dbo].[S_User] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [loginName] varchar(20)  NOT NULL,
    [password] varchar(20)  NOT NULL,
    [realName] varchar(20)  NULL,
    [sex] bit  NULL,
    [tel] varchar(20)  NULL,
    [email] varchar(50)  NULL,
    [addTime] datetime  NULL,
    [roleID] int  NOT NULL
);
GO

-- Creating table 'OilData'
CREATE TABLE [dbo].[OilData] (
    [SID] int  NOT NULL,
    [Data] varbinary(max)  NOT NULL
);
GO

-- Creating table 'Specs'
CREATE TABLE [dbo].[Specs] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [AddTime] datetime  NOT NULL,
    [Contents] varbinary(max)  NULL,
    [MethodID] int  NOT NULL,
    [UserID] int  NOT NULL,
    [Result] varbinary(max)  NULL,
    [SamplePlace] varchar(100)  NULL,
    [SampleTime] datetime  NULL,
    [OilName] varchar(100)  NULL,
    [AnalyTime] datetime  NULL,
    [Remark] varchar(max)  NULL,
    [ResultType] int  NOT NULL
);
GO

-- Creating table 'Properties'
CREATE TABLE [dbo].[Properties] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(200)  NOT NULL,
    [Name1] varchar(200)  NULL,
    [Code] varchar(50)  NOT NULL,
    [Units] varchar(50)  NULL,
    [Idx] int  NOT NULL,
    [ColumnIdx] int  NOT NULL,
    [Value] float  NOT NULL,
    [TableID] int  NOT NULL,
    [Eps] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'S_Moudle'
ALTER TABLE [dbo].[S_Moudle]
ADD CONSTRAINT [PK_S_Moudle]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'AllMethod'
ALTER TABLE [dbo].[AllMethod]
ADD CONSTRAINT [PK_AllMethod]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'S_User'
ALTER TABLE [dbo].[S_User]
ADD CONSTRAINT [PK_S_User]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [SID] in table 'OilData'
ALTER TABLE [dbo].[OilData]
ADD CONSTRAINT [PK_OilData]
    PRIMARY KEY CLUSTERED ([SID] ASC);
GO

-- Creating primary key on [ID] in table 'Specs'
ALTER TABLE [dbo].[Specs]
ADD CONSTRAINT [PK_Specs]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Properties'
ALTER TABLE [dbo].[Properties]
ADD CONSTRAINT [PK_Properties]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------