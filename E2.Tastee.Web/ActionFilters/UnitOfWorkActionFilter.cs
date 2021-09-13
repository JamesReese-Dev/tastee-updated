using System;
using Microsoft.AspNetCore.Mvc.Filters;
using E2.Tastee.Contracts.Services.Interfaces;
using Serilog;

namespace E2.Tastee.Web.ActionFilters
{
    public class UnitOfWorkActionFilter : IActionFilter
    {
        private readonly IUnitOfWork _unitOfWork;
        private const string RollbackExceptionMessage = "There was an error rolling the unit of work transaction back";

        public UnitOfWorkActionFilter(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _unitOfWork.Begin();
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception == null)
            {
                try
                {
                    _unitOfWork.Commit();
                }
                catch (Exception commitException)
                {
                    Log.Error(commitException,"There was an error committing the unit of work transaction");
                    try
                    {
                        _unitOfWork.Rollback();
                    }
                    catch (Exception rollbackException)
                    {
                        Log.Error(rollbackException, RollbackExceptionMessage);
                    }
                }
            }
            else
            {
                try
                {
                    _unitOfWork.Rollback();
                }
                catch (Exception rollbackException)
                {
                    Log.Error(rollbackException, RollbackExceptionMessage);
                }
            }
            _unitOfWork.Dispose();
        }
    }
}