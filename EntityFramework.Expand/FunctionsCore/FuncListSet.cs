using EntityFramework.Expand.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

//Entity Model Funcs Core
namespace EntityFramework.Expand.FunctionsCore
{
    /// <summary>
    /// 提供建立回應SQL函數列舉結果的剖析器
    /// </summary>
    /// <typeparam name="TEntity">預存的個體</typeparam>
    /// <typeparam name="Tout">輸出的列舉</typeparam>
    /// <exception cref="EntityFunctionsAttributeException">當調用預存個體前，發生的例外</exception>
    public class FuncListSet<TEntity, Tout> : ITableValuedFunctions<TEntity, Tout>, IEnumerable<Tout>, IEnumerable
        where TEntity : class, new()
        where Tout : class
    {
        private DbContext _contextModel;

        /// <summary>
        /// 提供建立回應SQL函數列舉結果的剖析器
        /// </summary>
        /// <param name="contextModel">Entity執行個體</param>
        public FuncListSet(DbContext contextModel)
        {
            // TODO: Complete member initialization
            this.ObjectState = false;
            this._contextModel = contextModel;
        }

        #region ITableValuedFunctions

        /// <summary>
        /// 預存參數
        /// </summary>
        private TEntity _Parameter { get; set; }
        ///// <summary>
        ///// 設定預存參數
        ///// </summary>
        //public TEntity Parameter
        //{
        //    set
        //    {
        //        this.ObjectState = false;
        //        this._Parameter = value;
        //    }
        //}

        /// <summary>
        /// 執行結果回應的物件集合
        /// </summary>
        private IEnumerator<Tout> _RuntimeSource { get; set; }

        /// <summary>
        /// 使用內定義的方式執行，此時以Parameter屬性方式賦值的內容將會被覆蓋
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public ITableValuedFunctions<TEntity, Tout> Set(Action<TEntity> selector)
        {
            this._Parameter = new TEntity();
            selector.Invoke(this._Parameter);
            this.RunIt();
            //return int.Parse(returnCode.Value == null ? "-1" : returnCode.Value.ToString());
            return this;
        }

        /// <summary>
        /// 使用外定義的方式執行，此時以Parameter屬性作為設定值
        /// </summary>
        /// <returns></returns>
        public ITableValuedFunctions<TEntity, Tout> Set(TEntity parameter)
        {
            if (parameter == null)
                throw new EntityFunctionsAttributeException(typeof(TEntity).Name + "函數參數尚未設定，無法執行並取得執行結果的列舉值，執行Set()請設定Parameter才能取用列舉。");

            this._Parameter = parameter;

            this.RunIt();
            return this;
        }
        private void RunIt()
        {

            /*
        exec sp_executesql N'SELECT
        [Extent1].*
        FROM [dbo].[GetPrecisionMarketingUsers](@p0, @p1, @p2, @p3, @p4, @p5)  AS [Extent1]',N'@p0 tinyint, @p1 int, @p2 tinyint, @p3 tinyint, @p4 decimal(19,2), @p5 int'
        ,@p0 = null,@p1 = null,@p2 = null,@p3 = null,@p4 = null,@p5 =  null
            */


            //0:sp name 1:pars
            StringBuilder sbPar = new StringBuilder();

            Dictionary<string, System.Data.SqlClient.SqlParameter> sqlParameters = new Dictionary<string, System.Data.SqlClient.SqlParameter>();
            System.Data.SqlClient.SqlParameter sqlParameter = null;

            var properties = this._Parameter.GetType().GetProperties();
            int totalCount = properties.Count(), nowCount = 0;
            foreach (var p in properties)
            {
                nowCount++;

                EntityFunctionsAttribute CustType = (EntityFunctionsAttribute)Attribute.GetCustomAttribute(p, typeof(EntityFunctionsAttribute));

                object value = p.GetValue(this._Parameter, null);

                if (CustType != null)
                {
                    sqlParameter = null;
                    sbPar.Append(this.CreateParameters(value, p, CustType, out sqlParameter));
                    if (sqlParameter == null)
                    {
                        throw new EntityFunctionsAttributeException("找不到SqlParameter，產生函數參數時發生錯誤 屬性名稱：" + p.Name);
                    }
                    else
                    {
                        sqlParameters.Add(p.Name, sqlParameter);
                        if (nowCount < totalCount)
                        {
                            sbPar.Append(", ");
                        }
                    }
                }
            }

            string schema = "dbo";
            string execCommand = string.Format(@"SELECT * FROM [{0}].[{1}]({2})",
                schema, this._Parameter.GetType().Name, sbPar.ToString());

            var inPar = sqlParameters.Values.ToArray();
            this._RuntimeSource = this._contextModel.Database.SqlQuery<Tout>(execCommand, inPar).ToList().GetEnumerator();
            
            this.ObjectState = true;
        }

        private string CreateParameters(object value, System.Reflection.MemberInfo p, EntityFunctionsAttribute custType, out System.Data.SqlClient.SqlParameter sqlParameter)
        {
            string resultCmd = string.Empty;
            string parameterName = string.Empty;

            #region ActionContent
            //判斷如果value是NULL預設值時，給Null值
            if (custType.DefaultOrNullValue != null && value != null)
            {
                if (value.ToString() == custType.DefaultOrNullValue.ToString())
                {
                    value = null;
                }
            }


            System.Data.SqlDbType PropertyType = System.Data.SqlDbType.NVarChar;
            if (custType != null)
            {
                int length = 0;
                if (custType.Size.HasValue)
                {
                    var hasLen = int.TryParse(custType.Size.Value.ToString(), out length);
                    if (hasLen)
                    {
                        if (value != null)
                        {
                            if (value.ToString().Length > length)
                            {
                                value = value.ToString().Substring(0, length);
                            }
                        }
                    }
                }
                //取得
                PropertyType = (System.Data.SqlDbType)custType.AttrType;



                sqlParameter = new System.Data.SqlClient.SqlParameter();
                sqlParameter.SqlDbType = PropertyType;

                if (custType.Size.HasValue)
                {
                    sqlParameter.Size = custType.Size.Value;
                }
                if ((
                       (custType.Direction.HasValue &&
                           (custType.Direction == System.Data.ParameterDirection.Input ||
                            custType.Direction == System.Data.ParameterDirection.InputOutput
                           )
                       )
                       ||
                       custType.Direction.HasValue == false
                   ))
                {
                    if (value == null)
                    {
                        sqlParameter.Value = DBNull.Value;
                        sqlParameter.IsNullable = true;
                    }
                    else
                    {
                        sqlParameter.Value = value;
                    }

                }

                if (string.IsNullOrWhiteSpace(custType.ParameterName) == false)
                {
                    parameterName = sqlParameter.ParameterName = custType.ParameterName.Replace("@", string.Empty);
                }
                else
                {
                    parameterName = sqlParameter.ParameterName = p.Name;
                }

                string Cmd = " @{0} ";
                resultCmd = string.Format(Cmd, parameterName);

            }
            else
            {
                throw new EntityFunctionsAttributeException(p.Name + "屬性找不到EntityFunctionsAttribute，請確定是否已定義此設定。");
            }


            #endregion

            return resultCmd;
        }

        #endregion

        public IEnumerator<Tout> GetEnumerator()
        {
            if (this._Parameter == null)
                throw new EntityFunctionsAttributeException(typeof(TEntity).Name + "函數參數尚未設定，無法執行並取得執行結果的列舉值，請先設定Parameter再執行Set()後才能取用列舉。");
            if (this._RuntimeSource == null)
                throw new EntityFunctionsAttributeException(typeof(TEntity).Name + "函數尚未執行，無法取得執行結果的列舉值，請先執行Set()再叫用列舉。");
            if (this.ObjectState == false)
                throw new Exception("從上一次變更後尚未執行Set()結果");
            else
                return this._RuntimeSource;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (this._Parameter == null)
                throw new EntityFunctionsAttributeException(typeof(TEntity).Name + "函數參數尚未設定，無法執行並取得執行結果的列舉值，請先設定Parameter再執行Set()後才能取用列舉。");
            if (this._RuntimeSource == null)
                throw new EntityFunctionsAttributeException(typeof(TEntity).Name + "函數尚未執行，無法取得執行結果的列舉值，請先執行Set()再叫用列舉。");
            else
                return this._RuntimeSource;
        }


        bool ObjectState = false;
    }
}
