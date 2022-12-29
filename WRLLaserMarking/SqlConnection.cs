using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data.SQLite;
using System.Transactions;

namespace WRLLaserMarking
{
    public class SqlConnection
    {

        public static  List<MasterModel> GetModelDetails(string ProductCode)
        {
            StringBuilder strq = new StringBuilder();
            strq.Append(string.Concat("select a.ProductCode,a.ProductName,a.Type,a.Alias,a.AssetImage,b.AssetVariable,a.ImageName from ProductMaster a ",
                                      " inner join ProductVariables b on a.ProductCode = b.ProductCode",
                                      " where a.ProductCode = '", ProductCode, "'"));
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(AppConnections.MarkingDb))
                {
                    var value = cnn.Query<MasterModel>(strq.ToString());
                    return value.ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<MasterModel>();
            }

        }
        public static bool SetModelDetails(MasterModel inputValues)
        {

            StringBuilder InsertModel = new StringBuilder();
            StringBuilder InsertVariable = new StringBuilder();
            InsertModel.Append(string.Concat("insert into ProductMaster(ProductCode,ProductName,Alias,Type,AssetImage,ImageName)",
                                            " values(@ProductCode, @ProductName, @Alias, @Type, @AssetImage, @ImageName)"));

            InsertVariable.Append(string.Concat("insert into productVariables(ProductCode, AssetVariable)",
                                                " values(@ProductCode, @AssetVariable)"));
             using(IDbConnection cnn=new SQLiteConnection(AppConnections.MarkingDb))
            {
               
                try
                {

                    int result1,result2;
                    using(var transacation=new TransactionScope())
                    {
                        result2 = cnn.Execute(InsertVariable.ToString(), inputValues);
                        if(result2!=0){ 
                        transacation.Complete();
                        }
                    }

                    using(var transacation2=new TransactionScope())
                    {
                        result1 = cnn.Execute(InsertModel.ToString(), inputValues);
                        if (result1 != 0) { 
                        transacation2.Complete();

                        }

                    }

                    if (result1==0 || result2==0)
                     {
               
                        return false;
                     }
                
                    return true;
                }
                catch (Exception ex)
                {
                   
                    return false;
                }

            }
           
        }


    }
    public class AppConnections
    {
        public static string MarkingDb => ConfigurationManager.ConnectionStrings["MarkingDb"].ConnectionString.ToString();
    }
}
