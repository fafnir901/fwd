using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects.Absrtact;
using FWD.DAL.Entities;

namespace FWD.DAL.Model
{
	public class ArticleContext : DbContext
	{
		public DbSet<Article> Articles { get; set; }
		public DbSet<EmbdedImage> EmbdedImages { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<ArticleGroup> Groups { get; set; }

		public DbSet<Comment> Comments { get; set; }

		public DbSet<TransactionLog> TransactionLogs { get; set; }
		public DbSet<Tag> Tags { get; set; }

		public DbSet<Plan> Plans { get; set; } 

		public ArticleContext() : base("name=Article")
		{
			Database.SetInitializer(new ArticleContextInitializer());
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ArticleGroup>()
			.Property(c => c.GroupName)
			.IsRequired();

			modelBuilder.Entity<Article>()
			.Property(s => s.ArticleName)
			.IsRequired();

			modelBuilder.Entity<Article>()
			.Property(s => s.InitialText)
			.IsRequired();

			modelBuilder.Entity<Article>()
			.Property(s => s.Link)
			.IsRequired();

			modelBuilder.Entity<User>()
			.Property(s => s.FirstName)
			.IsRequired();

			modelBuilder.Entity<User>()
			.Property(s => s.LastName)
			.IsRequired();

			modelBuilder.Entity<User>()
			.Property(s => s.Email)
			.IsRequired();

			modelBuilder.Entity<Comment>().Property(c => c.CommentId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			
		}
	}
}
