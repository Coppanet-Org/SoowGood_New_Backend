SET IDENTITY_INSERT [dbo].[SgSpecialities] ON 

INSERT [dbo].[SgSpecialities] ([Id], [SpecialityName], [Description], [CreationTime], [CreatorId], [LastModificationTime], [LastModifierId], [IsDeleted], [DeleterId], [DeletionTime]) VALUES (2, N'General Physician', N'General Physician', CAST(N'2023-07-16T10:50:55.0453756' AS DateTime2), NULL, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[SgSpecialities] ([Id], [SpecialityName], [Description], [CreationTime], [CreatorId], [LastModificationTime], [LastModifierId], [IsDeleted], [DeleterId], [DeletionTime]) VALUES (4, N'Dentistry', N'Dentistry', CAST(N'2023-07-16T10:50:55.0453756' AS DateTime2), NULL, NULL, NULL, 0, NULL, NULL)
SET IDENTITY_INSERT [dbo].[SgSpecialities] OFF
GO
SET IDENTITY_INSERT [dbo].[SgSpecializations] ON 

INSERT [dbo].[SgSpecializations] ([Id], [SpecialityId], [SpecializationName], [Description], [CreationTime], [CreatorId], [LastModificationTime], [LastModifierId], [IsDeleted], [DeleterId], [DeletionTime]) VALUES (1, 2, N'General Physician', NULL, CAST(N'2023-07-16T10:50:55.0453756' AS DateTime2), NULL, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[SgSpecializations] ([Id], [SpecialityId], [SpecializationName], [Description], [CreationTime], [CreatorId], [LastModificationTime], [LastModifierId], [IsDeleted], [DeleterId], [DeletionTime]) VALUES (2, 4, N'Dentistry', NULL, CAST(N'2023-07-16T10:50:55.0453756' AS DateTime2), NULL, NULL, NULL, 0, NULL, NULL)
SET IDENTITY_INSERT [dbo].[SgSpecializations] OFF
GO
SET IDENTITY_INSERT [dbo].[SgDegrees] ON 

INSERT [dbo].[SgDegrees] ([Id], [DegreeName], [Description], [CreationTime], [CreatorId], [LastModificationTime], [LastModifierId], [IsDeleted], [DeleterId], [DeletionTime]) VALUES (1, N'MBBS', NULL, CAST(N'2023-07-16T10:50:55.0453756' AS DateTime2), NULL, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[SgDegrees] ([Id], [DegreeName], [Description], [CreationTime], [CreatorId], [LastModificationTime], [LastModifierId], [IsDeleted], [DeleterId], [DeletionTime]) VALUES (2, N'BDS', NULL, CAST(N'2023-07-16T10:50:55.0453756' AS DateTime2), NULL, NULL, NULL, 0, NULL, NULL)
SET IDENTITY_INSERT [dbo].[SgDegrees] OFF
GO
