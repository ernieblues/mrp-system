using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using MrpSystem.Server.Authorization;
using MrpSystem.Server.Models;
using MrpSystem.Server.Models.Purchasing;
using MrpSystem.Server.Models.Purchasing.Enums;

namespace MrpSystem.Server.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string seedUserPW)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            var adminId = await EnsureUser(serviceProvider, seedUserPW, "admin@mail.com");
            await EnsureRole(serviceProvider, adminId, Constants.SystemAdministratorsRole);

            var directorId = await EnsureUser(serviceProvider, seedUserPW, "director@mail.com");
            await EnsureRole(serviceProvider, directorId, Constants.PurchasingDirectorsRole);

            var managerId = await EnsureUser(serviceProvider, seedUserPW, "manager@mail.com");
            await EnsureRole(serviceProvider, managerId, Constants.PurchasingManagersRole);

            var user1Id = await EnsureUser(serviceProvider, seedUserPW, "user1@mail.com");
            var user2Id = await EnsureUser(serviceProvider, seedUserPW, "user2@mail.com");
            var user3Id = await EnsureUser(serviceProvider, seedUserPW, "user3@mail.com");

            var userIds = new[] { adminId, directorId, managerId, user1Id, user2Id, user3Id };

            await SeedDB(context, userIds);
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string seedUserPW, string email)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, seedUserPW);

                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create seed user '{email}': {errors}");
                }
            }

            return user.Id;
        }

        private static async Task EnsureRole(IServiceProvider serviceProvider, string uid, string role)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                var roleResult = await roleManager.CreateAsync(new ApplicationRole(role));

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create role '{role}': {errors}");
                }
            }

            var user = await userManager.FindByIdAsync(uid)
                ?? throw new Exception($"User with ID '{uid}' not found.");

            if (!await userManager.IsInRoleAsync(user, role))
            {
                var addToRoleResult = await userManager.AddToRoleAsync(user, role);

                if (!addToRoleResult.Succeeded)
                {
                    var errors = string.Join("; ", addToRoleResult.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to add user '{user.Email}' to role '{role}': {errors}");
                }
            }
        }

        public static async Task SeedDB(ApplicationDbContext context, string[] userIds)
        {
            if (context.PurchaseRequisitions.Any())
            {
                return;
            }

            var costCenters = new[]
            {
                new CostCenter
                {
                    Code = "ENG",
                    Name = "Engineering",
                },
                new CostCenter
                {
                    Code = "IT",
                    Name = "Information Technology",
                },
                new CostCenter
                {
                    Code = "MFG",
                    Name = "Manufacturing",
                },
            };

            var vendors = new[]
            {
                new Vendor
                {
                    Code = "ACME",
                    Name = "Acme Corporation",
                    Email = "sales@acme.com",
                },
                new Vendor
                {
                    Code = "GIS",
                    Name = "Global Office Supply",
                    Email = "sales@gis.com",
                },
                new Vendor
                {
                    Code = "PPC",
                    Name = "Precision Parts Co.",
                    Email = "sales@ppc.com",
                },
            };

            context.CostCenters.AddRange(costCenters);
            context.Vendors.AddRange(vendors);
            await context.SaveChangesAsync(); // get IDs

            var purchaseRequisitions = new[]
            {
                new PurchaseRequisition
                {
                    RequestedById = userIds[3],
                    DateRequested = DateOnly.FromDateTime(DateTime.Now),
                    DateRequired = DateOnly.FromDateTime(DateTime.Now),
                    CostCenterId = costCenters[0].Id,
                    VendorId = vendors[0].Id,
                    Comments = "ACME product evaluation",
                    Items = new List<PurchaseRequisitionItem>
                    {
                        new PurchaseRequisitionItem
                        {
                            LineNumber = 1,
                            Description = "Rocket Skates",
                            Quantity = 1,
                            UnitOfMeasure = "EA",
                            UnitPrice = 499.99m,
                        },
                        new PurchaseRequisitionItem
                        {
                            LineNumber = 2,
                            Description = "Anvil (Heavy Duty)",
                            Quantity = 3,
                            UnitOfMeasure = "EA",
                            UnitPrice = 129.50m,
                        },
                        new PurchaseRequisitionItem
                        {
                            LineNumber = 3,
                            Description = "Giant Rubber Band",
                            Quantity = 2,
                            UnitOfMeasure = "EA",
                            UnitPrice = 39.95m,
                        },
                    },
                    Approvals = new List<PurchaseRequisitionApproval>
                    {
                        new PurchaseRequisitionApproval
                        {
                            ApprovedById = userIds[0],
                            ApprovalStatus = ApprovalStatus.Approved,
                            DateApproved = DateTime.UtcNow,
                            Comments = "",
                        },
                    },
                },
                new PurchaseRequisition
                {
                    RequestedById = userIds[4],
                    DateRequested = DateOnly.FromDateTime(DateTime.Now),
                    DateRequired = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
                    CostCenterId = costCenters[1].Id,
                    VendorId = vendors[1].Id,
                    Comments = "office equipment",
                    Items = new List<PurchaseRequisitionItem>
                    {
                        new PurchaseRequisitionItem
                        {
                            LineNumber = 1,
                            Description = "Laptop",
                            Quantity = 2,
                            UnitOfMeasure = "EA",
                            UnitPrice = 1199.99m,
                        },
                        new PurchaseRequisitionItem
                        {
                            LineNumber = 2,
                            Description = "Mouse",
                            Quantity = 5,
                            UnitOfMeasure = "EA",
                            UnitPrice = 24.99m,
                        },
                    },
                    Approvals = new List<PurchaseRequisitionApproval>
                    {
                        new PurchaseRequisitionApproval
                        {
                            ApprovedById = userIds[2],
                            ApprovalStatus = ApprovalStatus.Approved,
                            DateApproved = DateTime.UtcNow,
                            Comments = "Looks good.",
                        },
                        new PurchaseRequisitionApproval
                        {
                            ApprovedById = userIds[1],
                            ApprovalStatus = ApprovalStatus.Pending,
                            DateApproved = DateTime.UtcNow,
                            Comments = "",
                        },
                    },
                },
                new PurchaseRequisition
                {
                    RequestedById = userIds[5],
                    DateRequested = DateOnly.FromDateTime(DateTime.Now),
                    DateRequired = DateOnly.FromDateTime(DateTime.Now.AddDays(14)),
                    CostCenterId = costCenters[2].Id,
                    VendorId = vendors[2].Id,
                    Comments = "shop supplies",
                    Items = new List<PurchaseRequisitionItem>
                    {
                        new PurchaseRequisitionItem
                        {
                            LineNumber = 1,
                            Description = "Drill Press",
                            Quantity = 1,
                            UnitOfMeasure = "EA",
                            UnitPrice = 899.50m,
                        },
                        new PurchaseRequisitionItem
                        {
                            LineNumber = 2,
                            Description = "Safety Glasses",
                            Quantity = 10,
                            UnitOfMeasure = "EA",
                            UnitPrice = 9.75m,
                        },
                    },
                    Approvals = new List<PurchaseRequisitionApproval>
                    {
                        new PurchaseRequisitionApproval
                        {
                            ApprovedById = userIds[2],
                            ApprovalStatus = ApprovalStatus.Rejected,
                            DateApproved = DateTime.UtcNow,
                            Comments = "",
                        },
                        new PurchaseRequisitionApproval
                        {
                            ApprovedById = userIds[1],
                            ApprovalStatus = ApprovalStatus.Pending,
                            DateApproved = DateTime.UtcNow,
                            Comments = "",
                        },
                    },
                },
            };

            context.PurchaseRequisitions.AddRange(purchaseRequisitions);
            await context.SaveChangesAsync();
        }
    }
}