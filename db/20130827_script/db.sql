USE [testBO]
GO
/****** Object:  Table [dbo].[role]    Script Date: 08/27/2013 23:00:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[role](
	[idnum] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) NULL,
	[status] [tinyint] NULL,
	[isdelete] [tinyint] NULL,
	[permission] [nvarchar](max) NULL,
	[creater] [nvarchar](50) NULL,
	[createdate] [datetime] NULL,
	[modifier] [nvarchar](50) NULL,
	[modifydate] [datetime] NULL,
 CONSTRAINT [PK_role] PRIMARY KEY CLUSTERED 
(
	[idnum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[role_UpdateRoleData]    Script Date: 08/27/2013 23:00:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[role_UpdateRoleData]
	-- Add the parameters for the stored procedure here
	@idnum int,
	@name nvarchar(50),
	@status tinyint,
	@permission nvarchar(max) = '',
	@modifier  nvarchar(50) = ''
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE [testBO].[dbo].[role]
	SET [name] = @name
      ,[status] = @status
      ,[permission] = @permission
      ,[modifier] = @modifier
      ,[modifydate] = GETDATE()
      Where idnum = @idnum
      
  if (@@Rowcount = 0 )
  begin
	select 0 as StatusValue
  end
  else
  begin
	select 1 as StatusValue
  end
END
GO
/****** Object:  StoredProcedure [dbo].[role_GetRoleDataByID]    Script Date: 08/27/2013 23:00:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[role_GetRoleDataByID]
	-- Add the parameters for the stored procedure here
	@idnum int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [idnum]
	  ,[name]
	  ,[status]
	  ,[isdelete]
	  ,[permission]
	  ,[creater]
	  ,[createdate]
	  ,[modifier]
	  ,[modifydate]
	FROM [testBO].[dbo].[role]	with (nolock)
	Where isdelete = 0 and idnum = @idnum
END
GO
/****** Object:  StoredProcedure [dbo].[role_GetAllRoleData]    Script Date: 08/27/2013 23:00:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[role_GetAllRoleData]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [idnum]
	  ,[name]
	  ,[status]
	  ,[isdelete]
	  ,[permission]
	  ,[creater]
	  ,[createdate]
	  ,[modifier]
	  ,[modifydate]
	FROM [testBO].[dbo].[role]	with (nolock)
	Where isdelete = 0
END
GO
/****** Object:  StoredProcedure [dbo].[role_DeleteRoleDataByID]    Script Date: 08/27/2013 23:00:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[role_DeleteRoleDataByID]
	-- Add the parameters for the stored procedure here
	@idnum int,
	@modifier nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE [testBO].[dbo].[role]
	SET isdelete = 1
      ,[modifier] = @modifier
      ,[modifydate] = GETDATE()
      Where idnum = @idnum
END
GO
/****** Object:  StoredProcedure [dbo].[role_CreateRoleData]    Script Date: 08/27/2013 23:00:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[role_CreateRoleData]
	-- Add the parameters for the stored procedure here
	@name nvarchar(50),
	@status tinyint,
	@permission nvarchar(max) = '',
	@creater nvarchar(50)= ''

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO [testBO].[dbo].[role]
           ([name]
           ,[status]
           ,[permission]
           ,[creater])
     VALUES
           (
            @name
           ,@status
           ,@permission
           ,@creater)
           
  if (@@Rowcount = 0 )
  begin
	select 0 as StatusValue
  end
  else
  begin
	select 1 as StatusValue
  end
           
END
GO
/****** Object:  Default [DF_role_status]    Script Date: 08/27/2013 23:00:14 ******/
ALTER TABLE [dbo].[role] ADD  CONSTRAINT [DF_role_status]  DEFAULT ((1)) FOR [status]
GO
/****** Object:  Default [DF_role_isdelete]    Script Date: 08/27/2013 23:00:14 ******/
ALTER TABLE [dbo].[role] ADD  CONSTRAINT [DF_role_isdelete]  DEFAULT ((0)) FOR [isdelete]
GO
/****** Object:  Default [DF_role_createdate]    Script Date: 08/27/2013 23:00:14 ******/
ALTER TABLE [dbo].[role] ADD  CONSTRAINT [DF_role_createdate]  DEFAULT (getdate()) FOR [createdate]
GO
