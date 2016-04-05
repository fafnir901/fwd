using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FWD.BusinessObjects.Domain;
using FWD.DAL.Entities;
using FWD.DAL.Entities.Enums;
using FWD.DAL.Model;
using Article = FWD.BusinessObjects.Domain.Article;
using ArticleDb = FWD.DAL.Entities.Article;
using ArticleGroup = FWD.BusinessObjects.Domain.ArticleGroup;
using User = FWD.BusinessObjects.Domain.User;
using UserDb = FWD.DAL.Entities.User;
using Credential = FWD.BusinessObjects.Domain.Credential;
using Comment = FWD.BusinessObjects.Domain.Comment;
using CommentDb = FWD.DAL.Entities.Comment;
using ArticleGroupdDb = FWD.DAL.Entities.ArticleGroup;
using DomainTag = FWD.BusinessObjects.Domain.Tag;
using Plan = FWD.DAL.Entities.Plan;
using Tag = FWD.DAL.Entities.Tag;

namespace FWD.DAL.Mapping
{
	public static class Mapper
	{
		public static void ConfigureMapToDb()
		{
			AutoMapper.Mapper.CreateMap<Article, ArticleDb>()
				.ForMember(dest => dest.ArticleId, opt => opt.MapFrom(src => src.ArticleId))
				.ForMember(dest => dest.ArticleName, opt => opt.MapFrom(src => src.ArticleName))
				.ForMember(dest => dest.InitialText, opt => opt.MapFrom(src => src.InitialText))
				.ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.Link))
				.ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate))
				.ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.AuthorName))
				.ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.ArticleGroup))
				.ForMember(dest => dest.EmbdedImages, opt => opt.MapFrom(src => src.Images))
				.ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate))
				.ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));
			//.ForMember(dest => dest.UserArticleReferences, opt => opt.Ignore());

			AutoMapper.Mapper.CreateMap<Image, EmbdedImage>()
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data))
				.ForMember(dest => dest.ImageId, opt => opt.MapFrom(src => src.ImageId))
				.ForMember(dest => dest.Article, opt => opt.Ignore());

			AutoMapper.Mapper.CreateMap<DomainTag, Tag>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
				.ForMember(dest => dest.TagColor, opt => opt.MapFrom(src => src.TagColor))
				.ForMember(dest => dest.Articles, opt => opt.Ignore())
				.ForMember(dest => dest.TagType, opt => opt.MapFrom(src => src.TagType));

			AutoMapper.Mapper.CreateMap<ArticleGroup, ArticleGroupdDb>()
				.ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
				.ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.GroupName))
				.ForMember(dest => dest.Articles, opt => opt.MapFrom(src => src.Articles));

			AutoMapper.Mapper.CreateMap<User, UserDb>()
				.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
				.ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
				.ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Credential.Login))
				.ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Credential.Password))
				.ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => (int)src.UserRole))
				.ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar));
			//.ForMember(dest => dest.UserArticleReferences, opt => opt.Ignore());

			//AutoMapper.Mapper.CreateMap<Credential, CredentialDb>()
			//	.ForMember(dest => dest.CredentialId, opt => opt.MapFrom(src => src.CredentialId))
			//	.ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Login))
			//	.ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
			//	.ForMember(dest => dest.UserId, opt => opt.Ignore())
			//	.ForMember(dest => dest.User, opt => opt.Ignore());

			AutoMapper.Mapper.CreateMap<Comment, CommentDb>()
				.ForMember(dest => dest.CommentId, opt => opt.MapFrom(src => src.CommentId))
				.ForMember(dest => dest.CommentText, opt => opt.MapFrom(src => src.CommentText))
				.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
				.ForMember(dest => dest.AddedDate, opt => opt.MapFrom(src => src.AddedDate))
				.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
				.ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.GroupName));

			AutoMapper.Mapper.CreateMap<Transaction, TransactionLog>()
				.ForMember(dest => dest.ActionDateTime, opt => opt.MapFrom(src => src.ActionDateTime))
				.ForMember(dest => dest.ActionType, opt => opt.MapFrom(src => src.ActionTypeInt))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.EntityType, opt => opt.MapFrom(src => src.EntityTypeInt))
				.ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.EntityId))
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.Parameters, opt => opt.Ignore())
				.ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));


			AutoMapper.Mapper.CreateMap<CurrentPlan, Plan>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.IsDone, opt => opt.MapFrom(src => src.IsDone))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.PossibleChangeDate, opt => opt.MapFrom(src => src.PossibleChangeDate))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.AddedDate, opt => opt.MapFrom(src => src.AddedDate));

			AutoMapper.Mapper.AssertConfigurationIsValid();
		}

		public static void ConfigureMapToBo()
		{
			AutoMapper.Mapper.CreateMap<ArticleDb, Article>().
				ForMember(dest => dest.ArticleId, opt => opt.MapFrom(src => src.ArticleId))
				.ForMember(dest => dest.ArticleName, opt => opt.MapFrom(src => src.ArticleName))
				.ForMember(dest => dest.InitialText, opt => opt.MapFrom(src => src.InitialText))
				.ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.Link))
				.ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.AuthorName))
				.ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate))
				.ForMember(dest => dest.ArticleGroup, opt => opt.MapFrom(src => src.Group))
				.ForMember(dest => dest.HtmlText, opt => opt.Ignore())
				.ForMember(dest => dest.NonHtmlText, opt => opt.Ignore())
				.ForMember(dest => dest.GroupId, opt => opt.MapFrom(c => c.Group.GroupId))
				.ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate))
				.ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags))
				.ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.EmbdedImages));

			AutoMapper.Mapper.CreateMap<EmbdedImage, Image>()
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data))
				.ForMember(dest => dest.ImageId, opt => opt.MapFrom(src => src.ImageId));

			AutoMapper.Mapper.CreateMap<Tag, DomainTag>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
				.ForMember(dest => dest.TagColor, opt => opt.MapFrom(src => src.TagColor))
				.ForMember(dest => dest.TagType, opt => opt.MapFrom(src => src.TagType));

			AutoMapper.Mapper.CreateMap<ArticleGroupdDb, ArticleGroup>()
				.ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
				.ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.GroupName))
				.ForMember(dest => dest.Articles, opt => opt.MapFrom(src => src.Articles))
				.ForMember(dest => dest.Groups, opt => opt.Ignore());

			AutoMapper.Mapper.CreateMap<UserDb, User>()
				.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
				.ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
				.ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.Credential, opt => opt.MapFrom(src => new Credential
				{
					Login = src.Login,
					Password = src.Password
				}))
				.ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar))
				.ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => (UserRoleEnum)src.UserRole))
				.ForMember(dest => dest.ArticlesNamesList, opt => opt.Ignore())
				.ForMember(dest => dest.Articles, opt => opt.Ignore());

			AutoMapper.Mapper.CreateMap<CommentDb, Comment>()
				.ForMember(dest => dest.CommentId, opt => opt.MapFrom(src => src.CommentId))
				.ForMember(dest => dest.CommentText, opt => opt.MapFrom(src => src.CommentText))
				.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
				.ForMember(dest => dest.AddedDate, opt => opt.MapFrom(src => src.AddedDate))
				.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
				.ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.GroupName));

			AutoMapper.Mapper.CreateMap<TransactionLog, Transaction>()
				.ForMember(dest => dest.ActionDateTime, opt => opt.MapFrom(src => src.ActionDateTime))
				.ForMember(dest => dest.ActionType, opt => opt.MapFrom(src => Enum.GetName(typeof(ActionType), src.ActionType)))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.EntityId))
				.ForMember(dest => dest.EntityType, opt => opt.MapFrom(src => Enum.GetName(typeof(EntityType), src.EntityType)))
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.Parameters, opt => opt.MapFrom(src => src.Parameters))
				.ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

			AutoMapper.Mapper.CreateMap<Plan, CurrentPlan>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.IsDone, opt => opt.MapFrom(src => src.IsDone))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.PossibleChangeDate, opt => opt.MapFrom(src => src.PossibleChangeDate))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.AddedDate, opt => opt.MapFrom(src => src.AddedDate));
		}

		[Obsolete]
		public static IMapperConfiguration GetConfiguration()
		{
			var config = new MapperConfiguration(
						cfg =>
						{
							cfg.CreateMap<EmbdedImage, Image>()
								.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
								.ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data))
								.ForMember(dest => dest.ImageId, opt => opt.MapFrom(src => src.ImageId));

							cfg.CreateMap<Entities.Tag, DomainTag>()
								.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
								.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
								.ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
								.ForMember(dest => dest.TagColor, opt => opt.MapFrom(src => src.TagColor))
								.ForMember(dest => dest.TagType, opt => opt.MapFrom(src => src.TagType));

							cfg.CreateMap<Entities.ArticleGroup, ArticleGroup>()
								.ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
								.ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.GroupName))
								.ForMember(dest => dest.Articles, opt => opt.MapFrom(src => src.Articles))
								.ForMember(dest => dest.Groups, opt => opt.Ignore());

							cfg.CreateMap<Entities.Article, Article>()
								.ForMember(dest => dest.ArticleId, opt => opt.MapFrom(src => src.ArticleId))
								.ForMember(dest => dest.ArticleName, opt => opt.MapFrom(src => src.ArticleName))
								.ForMember(dest => dest.InitialText, opt => opt.MapFrom(src => src.InitialText))
								.ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.Link))
								.ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.AuthorName))
								.ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate))
								.ForMember(dest => dest.ArticleGroup, opt => opt.MapFrom(src => src.Group))
								.ForMember(dest => dest.HtmlText, opt => opt.Ignore())
								.ForMember(dest => dest.NonHtmlText, opt => opt.Ignore())
								.ForMember(dest => dest.GroupId, opt => opt.MapFrom(c => c.Group.GroupId))
								.ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate))
								.ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));
						});

			return config;
		}
	}
}
