using ApplicationDev.Modules.User.Entity;
using ApplicationDev.Modules.Admin.Entity;
using Microsoft.EntityFrameworkCore;
using ApplicationDev.Modules.Blogs.Entity;
using ApplicationDev.Modules.Votes.Entity;
using ApplicationDev.Modules.Comments.Entity;

public class MyAppDbContext : DbContext
{
	//Ensure to add the DbSet for each entity
	//For Migration to work
	public DbSet<UserEntity> Users { get; set; }
	public DbSet<AdminEntity> Admin { get; set; }
	public DbSet<BlogEntity> Blogs { get; set; }

	public DbSet<CommentsEntity> BlogComments { get; set; }
	public DbSet<VoteEntity> Votes { get; set; }


	public MyAppDbContext(DbContextOptions<MyAppDbContext> options)
		: base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.Entity<BlogEntity>().OwnsOne(b => b.PostUser); //Because it is not actual a existing table and only used as type
		modelBuilder.Entity<VoteEntity>().OwnsOne(b => b.VoteUser);
	}
}