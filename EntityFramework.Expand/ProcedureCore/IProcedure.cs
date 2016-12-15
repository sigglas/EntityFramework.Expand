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
    /// <typeparam name="TEntity">預存參數</typeparam>
    public interface IProcedure<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 設定預存實體
        /// </summary>
        TEntity Parameter { set; }
        /// <summary>
        /// 取得預存實體
        /// </summary>
        /// <returns></returns>
        TEntity GetParameter();
        /// <summary>
        /// 執行預存，透過Lambda方式設定參數
        /// </summary>
        /// <param name="selector">設定參數</param>
        /// <returns></returns>
        IProcedure<TEntity> Run(Action<TEntity> selector);

        /// <summary>
        /// 執行預存
        /// </summary>
        /// <returns></returns>
        int Run();


        /// <summary>
        /// 預存的執行結果回傳CODE 
        /// </summary>
        /// <returns>若為-1則可能是執行失敗</returns>
        int GetReturnCode();
    }
    /// <summary>
    /// 提供Entity model擴充預存程序的方法，可列舉
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    public interface IProcedure<TEntity, TSource> : IEnumerable<TSource>, IEnumerable
        where TEntity : class
        where TSource : class
    {
        /// <summary>
        /// 設定預存實體
        /// </summary>
        TEntity Parameter { set; }
        /// <summary>
        /// 取得預存實體
        /// </summary>
        /// <returns></returns>
        TEntity GetParameter();

        /// <summary>
        /// 執行預存，透過Lambda方式設定參數
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        IProcedure<TEntity, TSource> Run(Action<TEntity> selector);

        /// <summary>
        /// 執行預存
        /// </summary>
        /// <returns></returns>
        int Run();

        /// <summary>
        /// 預存的執行結果回傳CODE 
        /// </summary>
        /// <returns>若為-1則可能是執行失敗</returns>
        int GetReturnCode();
    }
}
