# EntityFramework.Expand
EntityFramework.Expand is extensions EF6.0,for easy to use MS-SQL stored procedure like the code-first projects. 

# Quickstart
Step1 : Create your EntityModel file like usp_helloworld.cs

    using EntityFramework.Expand.ProcedureCore;
    using System.Data;
      /*Sample*/
      public class usp_helloworld
      {
        [EntityProcedure(System.Data.SqlDbType.Int)]
        public int p1 { get; set; }
        [EntityProcedure(System.Data.SqlDbType.VarChar, 20, ParameterDirection.Output)]
        public string p2 { get; set; }

        public class Result
        {
            public int col1 { get; set; }
            public int col2 { get; set; }
        }
      }

Step2 : Create stored procedure on your database

    Create PROCEDURE [dbo].[usp_helloworld]
    -- Add the parameters for the stored procedure here
    @p1 int , 
    @p2 varchar(20) output
    AS
    BEGIN
      -- SET NOCOUNT ON added to prevent extra result sets from
	    -- interfering with SELECT statements.
	    SET NOCOUNT ON;

      -- Insert statements for procedure here
      set @p2='helloWorld value';
	
	    SELECT 1 as 'col1','hello1' as 'col2';
	    return 1
    END
    
Step3 : setup to your DbContext Model ,use SpListSet

    using EntityFramework.Expand.ProcedureCore;
    public partial class MyEntities : DbContext
    {
      public virtual SpListSet<usp_helloworld, usp_helloworld.Result> usp_helloworld { get { return new SpListSet<usp_helloworld, usp_helloworld.Result>(this); } }
    }
    
Step4 : use it!

            var context = new MyEntities();
            var obj = context.usp_helloworld.Run(par =>
            {
                par.p1 = 1;
            });

            //get ReturnCode from 'return 1' or other return
            var code = obj.GetReturnCode();
            
            //get list from table select 
            var myList = obj.ToList();
            
            //or search some one
            var myData = obj.Where(w => w.col1 == 1).ToList();

# The other
>SpValueSet is only Output value

    //DbContext
    public virtual SpValueSet<usp_helloworld> USP_HelloworldValue { get { return new SpValueSet<usp_helloworld>(this); } }
    ----------------
    //your code
    //after RUN() use GetParameter() to get p2(OutPut)
    var p = obj.GetParameter();
    string value = p.p2;
    
>
design : 2015-2017
