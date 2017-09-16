using System;
using System.Data;

namespace EntityFramework.Expand.FunctionsCore
{
    /// <summary>
    /// 提供 Entity FrameWork 預存擴增的實作方式
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityFunctionsAttribute : System.Attribute
    {
        internal ParameterDirection? Direction = ParameterDirection.Input;
        /// <summary>
        /// 取得或設定參數的 System.Data.SqlDbType。
        /// </summary>
        public virtual SqlDbType AttrType { get; set; }

        /// <summary>
        /// 定義參數名稱，若不定義則會直接使用屬性名
        /// </summary>
        public virtual string ParameterName { get; set; }

        /// <summary>
        /// 取得或設定資料行中資料的最大大小 (以位元組為單位)。
        /// </summary>
        public virtual int? Size { get; set; }
        /// <summary>
        /// 如果屬性值為Null時代用的值
        /// </summary>
        public virtual object DefaultOrNullValue { get; set; }

        /// <summary>
        /// 指示資料型態
        /// </summary>
        /// <param name="attrType">設定參數的 System.Data.SqlDbType。</param>
        public EntityFunctionsAttribute(SqlDbType attrType)
        {
            this.AttrType = attrType;
        }

        /// <summary>
        /// 指示資料型態及參數名稱
        /// </summary>
        /// <param name="attrType">設定參數的 System.Data.SqlDbType。</param>
        /// <param name="parameterName">定義參數名稱</param>
        public EntityFunctionsAttribute(SqlDbType attrType, string parameterName)
        {
            this.AttrType = attrType;
            this.ParameterName = parameterName;
        }

        /// <summary>
        /// 指示資料型態及長度
        /// </summary>
        /// <param name="attrType">設定參數的 System.Data.SqlDbType。</param>
        /// <param name="size">設定資料行中資料的最大大小 (以位元組為單位)。</param>
        public EntityFunctionsAttribute(SqlDbType attrType, int size)
        {
            this.AttrType = attrType;
            this.Size = size;
        }
        /// <summary>
        /// 指示資料型態及長度及參數名稱
        /// </summary>
        /// <param name="attrType">設定參數的 System.Data.SqlDbType。</param>
        /// <param name="size">設定資料行中資料的最大大小 (以位元組為單位)。</param>
        /// <param name="parameterName">定義參數名稱</param>
        public EntityFunctionsAttribute(SqlDbType attrType, int size, string parameterName)
        {
            this.AttrType = attrType;
            this.Size = size;
            this.ParameterName = parameterName;
        }
        
    }

    /// <summary>
    /// EntityFunctionsAttribute 例外
    /// </summary>
    public class EntityFunctionsAttributeException : Exception
    {
        /// <summary>
        /// EntityFunctionsAttribute 例外
        /// </summary>
        /// <param name="ex">例外敘述</param>
        public EntityFunctionsAttributeException(string ex)
            : base(ex)
        { 

        }
    }
}
