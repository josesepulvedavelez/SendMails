CREATE DATABASE empresas;
USE empresas;

CREATE TABLE Lista 
(
	[Nombre] [nvarchar](255) NULL,
	[Sector] [nvarchar](255) NULL,
	[Email] [nvarchar](255) NULL,
	[Web] [nvarchar](255) NULL,
	[Telefono] [nvarchar](255) NULL,
	[Direccion] [nvarchar](255) NULL,
	[Ciudad] [nvarchar](255) NULL,
	[EmailEnviado] [bit] NOT NULL,
	[FechaEnvio] [datetime] NULL,
	[Id] [int] IDENTITY(1,1) PRIMARY KEY
) 
