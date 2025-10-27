using Domain.Entities;
using Infrastructure.Persistences.Contexts.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Persistences.Contexts
{
    public partial class DbContextSystem : DbContext
    {
        public DbContextSystem(DbContextOptions<DbContextSystem> options) : base(options) { }

        public virtual DbSet<Actions> Actions { get; set; }
        public virtual DbSet<Brands> Brands { get; set; } 
        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<Consumers> Consumers { get; set; }
        public virtual DbSet<GoodsIssue> GoodsIssues { get; set; } = null!;
        public virtual DbSet<GoodsIssueDetails> GoodsIssuesDetails { get; set; }
        public virtual DbSet<GoodsReceipt> GoodsReceipt { get; set; }
        public virtual DbSet<GoodsReceiptDetails> GoodsReceiptDetails { get; set; }
        public virtual DbSet<InboundTransfer> InboundTransfer { get; set; }
        public virtual DbSet<InboundTransferDetails> InboundTransferDetails { get; set; }
        public virtual DbSet<Inventory> Inventory { get; set; }
        public virtual DbSet<Modules> Modules { get; set; }
        public virtual DbSet<OutboundTransfer> OutboundTransfer { get; set; }
        public virtual DbSet<OutboundTransferDetails> OutboundTransferDetails { get; set; }
        public virtual DbSet<Permissions> Permissions { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Stores> Stores { get; set; }
        public virtual DbSet<Suppliers> Suppliers { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasAnnotation("Relational:Collation", "Modern_Spanish_CI_AS");
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
