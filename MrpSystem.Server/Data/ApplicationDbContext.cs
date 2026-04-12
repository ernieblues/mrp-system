using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using MrpSystem.Server.Models;
using MrpSystem.Server.Models.Purchasing;

namespace MrpSystem.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>,
        ApplicationUserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }

        // Shared Data
        public DbSet<CostCenter> CostCenters { get; set; }

        // Purchasing
        public DbSet<PurchaseRequisition> PurchaseRequisitions { get; set; }
        public DbSet<PurchaseRequisitionApproval> PurchaseRequisitionApprovals { get; set; }
        public DbSet<PurchaseRequisitionItem> PurchaseRequisitionItems { get; set; }
        public DbSet<Vendor> Vendors { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ApplicationUserRole
            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            // Purchase Requisition
            builder.HasSequence<int>("PRNumberSeq")
                .StartsAt(1000)
                .IncrementsBy(1);

            builder.Entity<PurchaseRequisition>(pr =>
            {
                pr.Property(p => p.PurchaseRequisitionNumber)
                    .HasDefaultValueSql("NEXT VALUE FOR PRNumberSeq");

                pr.HasOne(p => p.RequestedBy)
                  .WithMany()
                  .HasForeignKey(p => p.RequestedById)
                  .OnDelete(DeleteBehavior.NoAction);

                pr.HasOne(p => p.CostCenter)
                  .WithMany(c => c.PurchaseRequisitions)
                  .HasForeignKey(p => p.CostCenterId)
                  .OnDelete(DeleteBehavior.Restrict);

                pr.HasOne(p => p.Vendor)
                  .WithMany(v => v.PurchaseRequisitions)
                  .HasForeignKey(p => p.VendorId)
                  .OnDelete(DeleteBehavior.Restrict);

                pr.HasMany(p => p.Items)
                  .WithOne(i => i.PurchaseRequisition)
                  .HasForeignKey(i => i.PurchaseRequisitionId)
                  .OnDelete(DeleteBehavior.Cascade);

                pr.HasMany(p => p.Approvals)
                  .WithOne(a => a.PurchaseRequisition)
                  .HasForeignKey(a => a.PurchaseRequisitionId)
                  .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<PurchaseRequisitionApproval>(pa =>
            {
                pa.HasOne(a => a.ApprovedBy)
                  .WithMany()
                  .HasForeignKey(a => a.ApprovedById)
                  .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
