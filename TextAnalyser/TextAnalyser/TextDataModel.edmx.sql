
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 06/07/2016 10:35:53
-- Generated from EDMX file: E:\Proeqti\Repo\TextAnalyser\TextAnalyser\TextAnalyser\TextDataModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [TextDataStore];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[CategorisedText]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CategorisedText];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'CategorisedTexts'
CREATE TABLE [dbo].[CategorisedTexts] (
    [Id] int  NOT NULL,
    [Name] nvarchar(50)  NULL,
    [Content] nvarchar(max)  NULL,
    [Category] smallint  NOT NULL
);
GO

-- Creating table 'FilteredTexts'
CREATE TABLE [dbo].[FilteredTexts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Content] nvarchar(max)  NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Category] smallint  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'CategorisedTexts'
ALTER TABLE [dbo].[CategorisedTexts]
ADD CONSTRAINT [PK_CategorisedTexts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FilteredTexts'
ALTER TABLE [dbo].[FilteredTexts]
ADD CONSTRAINT [PK_FilteredTexts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------