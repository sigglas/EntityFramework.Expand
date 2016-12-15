using EntityFramework.Expand.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

//Entity Model Sp Core
namespace EntityFramework.Expand.ProcedureCore
{

    /// <summary>
    /// 提供建立以值為回傳結果的SQL預存程序的剖析器
    /// </summary>
    /// <typeparam name="TEntity">要執行的預存個體</typeparam>
    /// <exception cref="EntityProcedureAttributeException">當調用預存個體前，發生的例外</exception>
    public class SpValueSet<TEntity> : IProcedure<TEntity> where TEntity : class, new()
    {
        private DbContext _contextModel;

        /// <summary>
        /// 建立以值為回傳結果的SQL預存程序
        /// </summary>
        /// <param name="contextModel">Entity執行個體</param>
        public SpValueSet(DbContext contextModel)
        {
            // TODO: Complete member initialization
            this.ObjectState = false;
            this._contextModel = contextModel;
        }

        /// <summary>
        /// 預存預設回應的方式
        /// </summary>
        private int _returnCode { get; set; }

        /// <summary>
        /// 預存參數
        /// </summary>
        private TEntity _Parameter { get; set; }
        /// <summary>
        /// 設定預存參數
        /// </summary>
        public TEntity Parameter
        {
            set
            {
                this.ObjectState = false;
                this._Parameter = value;
            }
        }

        /// <summary>
        /// 取得執行結果
        /// </summary>
        /// <returns></returns>
        public TEntity GetParameter()
        {
            if (this.ObjectState == false)
                throw new Exception("從上一次變更後尚未執行Run()結果");
            return this._Parameter.Clone();
        }

        /// <summary>
        /// 使用內定義的方式執行，此時以Parameter屬性方式賦值的內容將會被覆蓋
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public IProcedure<TEntity> Run(Action<TEntity> selector)
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
        public int Run()
        {
            if (this._Parameter == null)
                throw new EntityProcedureAttributeException(typeof(TEntity).Name + "預存程序參數尚未設定，無法執行並取得執行結果的列舉值，請先設定Parameter再執行Run()後才能取用列舉。");

            this.RunIt();
            return this._returnCode;
        }
        private void RunIt()
        {
            //0:sp name 1:pars
            StringBuilder sbPar = new StringBuilder();

            Dictionary<string, System.Data.SqlClient.SqlParameter> sqlParameters = new Dictionary<string, System.Data.SqlClient.SqlParameter>();
            System.Data.SqlClient.SqlParameter sqlParameter = null;

            var properties = this._Parameter.GetType().GetProperties();
            int totalCount = properties.Count(), nowCount = 0;
            foreach (var p in properties)
            {
                nowCount++;

                EntityProcedureAttribute CustType = (EntityProcedureAttribute)Attribute.GetCustomAttribute(p, typeof(EntityProcedureAttribute));

                object value = p.GetValue(this._Parameter, null);

                if (CustType != null)
                {
                    sqlParameter = null;
                    sbPar.Append(this.CreateParameters(value, p, CustType, out sqlParameter));
                    if (sqlParameter == null)
                    {
                        throw new EntityProcedureAttributeException("找不到SqlParameter，產生預存程序參數時發生錯誤 屬性名稱：" + p.Name);
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

            var returnCode = new System.Data.SqlClient.SqlParameter();
            returnCode.ParameterName = "@Return_value";
            returnCode.SqlDbType = System.Data.SqlDbType.Int;
            returnCode.Direction = System.Data.ParameterDirection.Output;
            sqlParameters.Add("Return_value", returnCode);

            string execCommand =  string.Format(@"EXEC @Return_value = {0} {1}", this._Parameter.GetType().Name, sbPar.ToString());

            var inPar = sqlParameters.Values.ToArray();
            var runWork = this._contextModel.Database.SqlQuery<TEntity>(execCommand, inPar).ToList();

            sqlParameters.ToList().ForEach(item =>
            {
                switch (item.Value.Direction)
                {
                    case System.Data.ParameterDirection.InputOutput:
                    case System.Data.ParameterDirection.Output:
                    case System.Data.ParameterDirection.ReturnValue:
                        foreach (var p in this._Parameter.GetType().GetProperties())
                        {
                            if (p.Name == item.Key)
                            {
                                var value = item.Value.Value;
                                var t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                                var safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                                p.SetValue(this._Parameter, safeValue);
                            }
                        }
                        break;
                }
            });
            this._returnCode = int.Parse(returnCode == null ? "-1" : returnCode.Value.ToString());

            this.ObjectState = true;
        }

        private string CreateParameters(object value, System.Reflection.MemberInfo p, EntityProcedureAttribute custType, out System.Data.SqlClient.SqlParameter sqlParameter)
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

                string Cmd = " @{0} = @{0} {1}";
                if (custType.Direction.HasValue)
                {
                    sqlParameter.Direction = custType.Direction.Value;
                    switch (custType.Direction.Value)
                    {
                        case System.Data.ParameterDirection.Output:
                        case System.Data.ParameterDirection.InputOutput:
                            resultCmd = string.Format(Cmd, parameterName, "Output");
                            break;
                        default:
                            resultCmd = string.Format(Cmd, parameterName, string.Empty);
                            break;
                    }
                }
                else
                {
                    resultCmd = string.Format(Cmd, parameterName, string.Empty);
                }

            }
            else
            {
                throw new EntityProcedureAttributeException(p.Name + "屬性找不到EntityProcedureAttribute，請確定是否已定義此設定。");
            }


            #endregion

            return resultCmd;
        }

        bool ObjectState = false;

        /// <summary>
        /// 預存的執行結果回傳CODE 
        /// </summary>
        /// <returns>若為-1則可能是執行失敗</returns>
        public int GetReturnCode()
        {
            if (this.ObjectState == false)
                throw new Exception("從上一次變更後尚未執行Run()結果");
            return _returnCode;
        }

    }
}
