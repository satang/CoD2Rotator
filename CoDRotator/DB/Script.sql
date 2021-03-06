USE [master]
GO
/****** Object:  Database [Rotator]    Script Date: 1/13/2016 2:38:45 PM ******/
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'Rotator')
BEGIN
CREATE DATABASE [Rotator] ON  PRIMARY 
( NAME = N'Rotator', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10.MSSQLSERVER\MSSQL\DATA\Rotator.mdf' , SIZE = 32000KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Rotator_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10.MSSQLSERVER\MSSQL\DATA\Rotator_log.LDF' , SIZE = 27200KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
END

GO
ALTER DATABASE [Rotator] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Rotator].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Rotator] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Rotator] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Rotator] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Rotator] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Rotator] SET ARITHABORT OFF 
GO
ALTER DATABASE [Rotator] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Rotator] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Rotator] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Rotator] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Rotator] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Rotator] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Rotator] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Rotator] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Rotator] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Rotator] SET  ENABLE_BROKER 
GO
ALTER DATABASE [Rotator] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Rotator] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Rotator] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Rotator] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Rotator] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Rotator] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Rotator] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Rotator] SET RECOVERY FULL 
GO
ALTER DATABASE [Rotator] SET  MULTI_USER 
GO
ALTER DATABASE [Rotator] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Rotator] SET DB_CHAINING OFF 
GO
EXEC sys.sp_db_vardecimal_storage_format N'Rotator', N'ON'
GO
USE [Rotator]
GO
/****** Object:  Table [dbo].[MapInfoes]    Script Date: 1/13/2016 2:38:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MapInfoes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MapInfoes](
	[MapInfoId] [int] IDENTITY(1,1) NOT NULL,
	[Package] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
	[Team1] [nvarchar](max) NULL,
	[Team2] [nvarchar](max) NULL,
	[Score] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Image] [varbinary](max) NULL,
 CONSTRAINT [PK_dbo.MapInfoes] PRIMARY KEY CLUSTERED 
(
	[MapInfoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ServerSettings]    Script Date: 1/13/2016 2:38:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ServerSettings]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ServerSettings](
	[ServerSettingId] [int] IDENTITY(1,1) NOT NULL,
	[Computer] [nvarchar](max) NULL,
	[Package] [nvarchar](max) NULL,
	[PackageDate] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.ServerSettings] PRIMARY KEY CLUSTERED 
(
	[ServerSettingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
USE [master]
GO
ALTER DATABASE [Rotator] SET  READ_WRITE 
GO


USE [Rotator]
GO
SET IDENTITY_INSERT [dbo].[ServerSettings] ON 

INSERT [dbo].[ServerSettings] ([ServerSettingId], [Computer], [Package], [PackageDate]) VALUES (1, N'CONFIG', N'//Bind Map rotate (backup)
bind P "map_rotate"


//Server Information
 
set sv_hostname "fumenla"
sets _Admin "gustavo"
sets _Location "Argentina"
 
 
//Server Options
 
set sv_allowDownload "1"
set sv_cheats "0"
set sv_floodProtect "0"
set sv_allowAnonymous "0"
set sv_voice "1"
 
 
//Network Options
 
 
 
//Game Options
 
set g_gametype "ctf"
set g_allowvote "1"
set scr_drawfriend "1"
set scr_forcerespawn "1"
set g_deadchat "0"
set scr_friendlyfire "3"
set scr_killcam "1"
set scr_spectatefree "0"
set scr_spectateenemy "0"
set scr_teambalance "2"
 
 
//Gametypes Configuration
 
set scr_dm_scorelimit "30"
set scr_dm_timelimit "15"
set scr_tdm_scorelimit "50"
set scr_tdm_timelimit "15"
set scr_hq_scorelimit "450"
set scr_hq_timelimit "30"
set scr_ctf_scorelimit "5"
set scr_ctf_timelimit "15"
set scr_sd_scorelimit "10"
set scr_sd_timelimit "0"
set scr_sd_graceperiod "15"
set scr_sd_roundlenght "4"
set scr_sd_roundlimit "0"
set scr_sd_bombtimer "60"
 
 
//Allowed Weapons
 
set scr_allow_m1carbine "1"
set scr_allow_m1garand "1"
set scr_allow_thompson "1"
set scr_allow_bar "1"
set scr_allow_greasegun "1"
set scr_allow_springfield "0"
set scr_allow_enfield "1"
set scr_allow_sten "1"
set scr_allow_bren "1"
set scr_allow_enfieldsniper "0"
set scr_allow_kar98k "1"
set scr_allow_mp40 "1"
set scr_allow_mp44 "1"
set scr_allow_kar98ksniper "0"
set scr_allow_g43 "1"
set scr_allow_nagant "1"
set scr_allow_pps42 "1"
set scr_allow_ppsh "1"
set scr_allow_nagantsniper "0"
set scr_allow_svt40 "0"
set scr_allow_shotgun "1"
set scr_allow_fraggrenades "1"
set scr_allow_smokegrenades "0"
', CAST(N'2013-04-03 00:00:00.000' AS DateTime))

SET IDENTITY_INSERT [dbo].[ServerSettings] OFF
