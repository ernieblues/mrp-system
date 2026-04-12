using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

using MrpSystem.Server.Models;
using MrpSystem.Server.Models.Purchasing;

namespace MrpSystem.Server.Authorization
{
    public class PurchaseRequisitionAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, PurchaseRequisition>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public PurchaseRequisitionAuthorizationHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            PurchaseRequisition resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            string? userId = _userManager.GetUserId(context.User);

            bool isSystemAdmin = context.User.IsInRole(Constants.SystemAdministratorsRole);
            bool isPurchasingDirector = context.User.IsInRole(Constants.PurchasingDirectorsRole);
            bool isPurchasingManager = context.User.IsInRole(Constants.PurchasingManagersRole);
            bool isRequester = resource.RequestedById == userId;

            // System administrators can do anything.
            if (isSystemAdmin)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Purchasing directors can do anything with purchase requisitions.
            if (isPurchasingDirector)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Purchasing managers can approve or reject.
            if ((requirement.Name == Constants.ApproveOperationName ||
                 requirement.Name == Constants.RejectOperationName) &&
                isPurchasingManager)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Requester can perform CRUD on their own requisition.
            if ((requirement.Name == Constants.CreateOperationName ||
                 requirement.Name == Constants.ReadOperationName ||
                 requirement.Name == Constants.UpdateOperationName ||
                 requirement.Name == Constants.DeleteOperationName) &&
                isRequester)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}