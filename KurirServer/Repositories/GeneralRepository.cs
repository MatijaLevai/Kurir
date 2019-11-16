using KurirServer.Intefaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Repositories
{

    public class GeneralRepository:IGeneralRepository
    {
        #region Dependency Injection
        private readonly KurirDbContext context;

        public GeneralRepository(KurirDbContext context)
        {
            this.context = context;
        }
        #endregion

        /// <summary>
        /// Adds defined entity into db
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void Add<T>(T entity) where T : class
        {
            context.Add(entity);

        }
        /// <summary>
        /// Deletes defined entity from db
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void Delete<T>(T entity) where T : class
        {
            context.Remove(entity);
        }
        /// <summary>
        /// Saves all changes in db
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                return (await context.SaveChangesAsync()) > 0;
            }
            catch (Exception ex)
            { Debug.WriteLine(ex.Message + ex.InnerException);
                return false;
            }
        }

        public async Task<bool> Update<T>(T entity) where T : class
        {
            try
            {
                context.Update<T>(entity);
                await SaveChangesAsync();
                return true;
            }
            catch { return false; }
            
        }

    }
}



