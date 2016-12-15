using System;
using System.Data;

namespace EntityFramework.Expand.ProcedureCore
{
    /// <summary>
    /// 提供 Entity FrameWork 預存擴增的實作方式
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityProcedureAttribute : System.Attribute
    {
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
        /// 取得或設定值，指出參數是否為只能輸入、只能輸出、雙向 (Bidirectional) 或預存程序 (Stored Procedure) 傳回值參數。
        /// </summary>
        public virtual ParameterDirection? Direction { get; set; }

        /// <summary>
        /// 指示資料型態
        /// </summary>
        /// <param name="attrType">設定參數的 System.Data.SqlDbType。</param>
        public EntityProcedureAttribute(SqlDbType attrType)
        {
            this.AttrType = attrType;
        }

        /// <summary>
        /// 指示資料型態及參數名稱
        /// </summary>
        /// <param name="attrType">設定參數的 System.Data.SqlDbType。</param>
        /// <param name="parameterName">定義參數名稱</param>
        public EntityProcedureAttribute(SqlDbType attrType, string parameterName)
        {
            this.AttrType = attrType;
            this.ParameterName = parameterName;
        }

        /// <summary>
        /// 指示資料型態及長度
        /// </summary>
        /// <param name="attrType">設定參數的 System.Data.SqlDbType。</param>
        /// <param name="size">設定資料行中資料的最大大小 (以位元組為單位)。</param>
        public EntityProcedureAttribute(SqlDbType attrType, int size)
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
        public EntityProcedureAttribute(SqlDbType attrType, int size, string parameterName)
        {
            this.AttrType = attrType;
            this.Size = size;
            this.ParameterName = parameterName;
        }

        /// <summary>
        /// 指示資料型態及長度及屬性是否為輸出(入)
        /// </summary>
        /// <param name="attrType">設定參數的 System.Data.SqlDbType。</param>
        /// <param name="direction">設定值，指出參數是否為只能輸入、只能輸出、雙向 (Bidirectional) 或預存程序 (Stored Procedure) 傳回值參數。</param>
        public EntityProcedureAttribute(SqlDbType attrType, ParameterDirection direction)
        {
            this.AttrType = attrType;
            this.Direction = direction;
        }

        /// <summary>
        /// 指示資料型態及屬性是否為輸出(入)及參數名稱
        /// </summary>
        /// <param name="attrType">設定參數的 System.Data.SqlDbType。</param>
        /// <param name="direction">設定值，指出參數是否為只能輸入、只能輸出、雙向 (Bidirectional) 或預存程序 (Stored Procedure) 傳回值參數。</param>
        /// <param name="parameterName">定義參數名稱</param>
        public EntityProcedureAttribute(SqlDbType attrType, ParameterDirection direction, string parameterName)
        {
            this.AttrType = attrType;
            this.Direction = direction;
            this.ParameterName = parameterName;
        }

        /// <summary>
        /// 指示資料型態及長度及屬性是否為輸出(入)
        /// </summary>
        /// <param name="attrType">設定參數的 System.Data.SqlDbType。</param>
        /// <param name="size">設定資料行中資料的最大大小 (以位元組為單位)。</param>
        /// <param name="direction">設定值，指出參數是否為只能輸入、只能輸出、雙向 (Bidirectional) 或預存程序 (Stored Procedure) 傳回值參數。</param>
        public EntityProcedureAttribute(SqlDbType attrType, int size, ParameterDirection direction)
        {
            this.AttrType = attrType;
            this.Size = size;
            this.Direction = direction;
        }

        /// <summary>
        /// 指示資料型態及長度及屬性是否為輸出(入)
        /// </summary>
        /// <param name="attrType">設定參數的 System.Data.SqlDbType。</param>
        /// <param name="size">設定資料行中資料的最大大小 (以位元組為單位)。</param>
        /// <param name="direction">設定值，指出參數是否為只能輸入、只能輸出、雙向 (Bidirectional) 或預存程序 (Stored Procedure) 傳回值參數。</param>
        /// <param name="parameterName">定義參數名稱</param>
        public EntityProcedureAttribute(SqlDbType attrType, int size, ParameterDirection direction, string parameterName)
        {
            this.AttrType = attrType;
            this.Size = size;
            this.Direction = direction;
            this.ParameterName = parameterName;
        }

        /// <summary>
        /// 指示資料型態及長度及屬性是否為輸出(入)，及預設值
        /// </summary>
        /// <param name="attrType">設定參數的 System.Data.SqlDbType。</param>
        /// <param name="size">設定資料行中資料的最大大小 (以位元組為單位)。</param>
        /// <param name="direction">設定值，指出參數是否為只能輸入、只能輸出、雙向 (Bidirectional) 或預存程序 (Stored Procedure) 傳回值參數。</param>
        /// <param name="parameterName">定義參數名稱</param>
        /// <param name="defaultOrNullValue">如果屬性值為Null時代用的值</param>
        public EntityProcedureAttribute(SqlDbType attrType, int size, ParameterDirection direction, string parameterName, object defaultOrNullValue)
        {
            this.AttrType = attrType;
            this.Size = size;
            this.Direction = direction;
            this.ParameterName = parameterName;
            this.DefaultOrNullValue = defaultOrNullValue;
        }

    }

    /// <summary>
    /// EntityProcedureAttribute 例外
    /// </summary>
    public class EntityProcedureAttributeException : Exception
    {
        /// <summary>
        /// EntityProcedureAttribute 例外
        /// </summary>
        /// <param name="ex">例外敘述</param>
        public EntityProcedureAttributeException(string ex)
            : base(ex)
        { 

        }
    }
}
