using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.Expand.Interface
{
    /// <summary>
    /// 提供Entity model擴充預存程序的方法，不含列舉行為
    /// </summary>
    /// <typeparam name="TEntity">參數</typeparam>
    public interface IScalarFunctions<TEntity, TValue>
        where TEntity : class
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter">設定參數</param>
        /// <returns></returns>
        TValue Get(TEntity parameter);
    }
    /// <summary>
    /// 提供Entity model擴充預存程序的方法，可列舉
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    public interface ITableValuedFunctions<TEntity, TSource> : IEnumerable<TSource>, IEnumerable
        where TEntity : class
        where TSource : class
    {
        /// <summary>
        /// 執行預存，透過Lambda方式設定參數
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        ITableValuedFunctions<TEntity, TSource> Set(Action<TEntity> selector);

        /// <summary>
        /// 執行預存
        /// </summary>
        /// <returns></returns>
        ITableValuedFunctions<TEntity, TSource> Set(TEntity parameter);
    }
}
