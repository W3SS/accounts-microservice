﻿using System;
using System.Linq;
using System.Threading.Tasks;
using tba.Accounts.Entities;
using tba.Core.Exceptions;
using tba.Core.Persistence.Interfaces;
using tba.Core.Services;
using tba.Core.Utilities;
using tba.accounts.Models;

namespace tba.accounts.Services
{
    public class AccountsService : EntityService<Account>, IAccountsService
    {

        public AccountsService(IRepository<Account> repository, ITimeProvider timeProvider)
            : base(repository, typeof(AccountsService).FullName, "account", timeProvider)
        {
        }

        /// <summary>
        /// Fetch an array of accounts for a given parent entity
        /// </summary>
        /// <param name="tenantId">a tenant</param>
        /// <param name="userId">a user</param>
        /// <param name="parentId">OPTIONAL - an account parent entity</param>
        /// <returns>array of account read-only models</returns>
        public async Task<AccountRm[]> FetchAsync(long tenantId, long userId, long? parentId = null)
        {
            var entities = await FetchEntitiesAsync(tenantId, userId, parentId, false);
            return AccountRm.From(entities).ToArray();
        }

        /// <summary>
        /// Insert a new account
        /// </summary>
        /// <param name="tenantId">a tenant</param>
        /// <param name="userId">a user</param>
        /// <param name="account"></param>
        /// <returns>a read-only account model</returns>
        public async Task<AccountRm> InsertAsync(long tenantId, long userId, AccountCm account)
        {
            var msg = "Insert" + ". " +
                       string.Format("tenantId={0}, userId={1}, account={2}", tenantId, userId, Serialization.Serialize(account));
            try
            {
                Log.Debug(msg);
                // create an entity
                var e = account.ToEntity(tenantId, DateTime.Now);
                await Repository.InsertAsync(userId, e);
                // return a account read-only view model
                return AccountRm.From(e);
            }
            catch (Exception exception)
            {
                Log.Error(msg, exception);
                throw new ApplicationException("Failed to insert " + account.Description + " " + FriendlyName);
            }
        }

        /// <summary>
        /// Update an existing account
        /// </summary>
        /// <param name="tenantId">a tenant</param>
        /// <param name="userId">a user</param>
        /// <param name="accountId">the id of the account to update</param>
        /// <param name="account">the account update model</param>
        /// <returns>a read-only account model</returns>
        public async Task<AccountRm> UpdateAsync(long tenantId, long userId, long accountId, AccountUm account)
        {
            var msg = "Update" + ". " +
                       string.Format("tenantId={0}, userId={1}, accountId={2}, account={3}", tenantId, userId, accountId, Serialization.Serialize(account));
            try
            {
                TenantCheck(tenantId, accountId, msg);
                Log.Debug(msg);
                // get the entity by id, we already know it exists for the client.
                var e = Repository.Get(accountId);
                // update entity
                account.UpdateEntity(e);
                await Repository.UpdateAsync(userId, e);
                // convert it to a read model
                return (AccountRm)AccountRm.From(e);
            }
            catch (EntityDoesNotExistException exception)
            {
                Log.Error(msg, exception);
                throw;
            }
            catch (Exception exception)
            {
                Log.Error(msg, exception);
                throw new ApplicationException("Failed to update the submitted " + FriendlyName);
            }
        }

        /// <summary>
        /// Mark and existing account as open (not complete)
        /// </summary>
        /// <param name="tenantId">a tenant</param>
        /// <param name="userId">a user</param>
        /// <param name="accountId">the id of the account to mark as open</param>
        /// <returns>a read-only account model</returns>
        public async Task<AccountRm> CloseAccount(long tenantId, long userId, long accountId)
        {
            throw new NotImplementedException();

            //var msg = "CloseAccount" + ". " +
            //           string.Format("tenantId={0}, userId={1}, accountId={2}", tenantId, userId, accountId);
            //try
            //{
            //    TenantCheck(tenantId, accountId, msg);
            //    Log.Debug(msg);
            //    // get the entity by id, we already know it exists for the client.
            //    var e = Repository.Get(accountId);
            //    // update entity
            //    e.CloseAccount(TimeProvider.UtcNow);
            //    await Repository.UpdateAsync(userId, e);
            //    // convert it to a read model
            //    return (AccountRm) AccountRm.From(e);
            //}
            //catch (Exception exception)
            //{
            //    Log.Error(msg, exception);
            //    throw new ApplicationException("Failed to close the submitted " + FriendlyName);
            //}
        }

        public AccountRm.AccountTypeRm[] GetTypes()
        {
            throw new NotImplementedException();
        }
    }
}
