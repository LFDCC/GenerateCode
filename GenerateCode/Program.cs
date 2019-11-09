using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlSugar;
using Generation.Codes;

namespace Generation
{
    /// <summary>
    /// F5直接运行生成项目
    /// </summary>
    /// <param name="args"></param>
    internal class Program
    {
        //如果你不需要自定义，直接配好数据库连接，F5运行项目
        private const SqlSugar.DbType dbType = SqlSugar.DbType.Oracle;

        /// <summary>
        /// 连接字符串
        /// </summary>
        //private const string connectionString = @"server=.\sqlexpress;uid=sa;pwd=ok;database=nj";       //sqlserver
        private const string connectionString = @"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=testdb)));User ID=scott;Password=ok";//oracle
        /// <summary>
        ///解决方案名称
        /// </summary>
        private const string SolutionName = "Testing";

        private const string ModelNamespace = SolutionName + ".Model";
        private const string ModelProjectName = SolutionName + ".Model";
        private const string ServiceNamespace = SolutionName + ".Service";
        private const string ServiceProjectName = SolutionName + ".Service";
        private const string RepositoryNamespace = SolutionName + ".Repository";
        private const string RepositoryProjectName = SolutionName + ".Repository";
        private const string IServiceNamespace = SolutionName + ".IService";
        private const string IServiceProjectName = SolutionName + ".IService";
        private const string IRepositoryNamespace = SolutionName + ".IRepository";
        private const string IRepositoryProjectName = SolutionName + ".IRepository";

        private static SqlSugarClient db;

        static TempParameter param = new TempParameter
        {
            IRepositoryNamespace = IRepositoryNamespace,
            IServiceNamespace = IServiceNamespace,
            ModelNamespace = ModelNamespace,
            RepositoryNamespace = RepositoryNamespace,
            ServiceNamespace = ServiceNamespace
        };
        /// <summary>
        /// 执行生成
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {

            /***创建解决方案***/
            Methods.CreateSln(SolutionName);

            /***连接数据库***/
            db = GetDB();

            /***生成实体***/
            GenerationClass();

            /***生成IBaseRepository***/
            GenerationIBaseRepository();

            /***生成IRepository***/
            GenerationIRepository();

            /***生成DbContext***/
            GenerationDContext();

            /***生成BaseRepository***/
            GenerationBaseRepository();

            /***生成Repository***/
            GenerationRepository();

            /***生成IBaseService***/
            GenerationIBaseService();

            /***生成IService***/
            GenerationIService();

            /***生成BaseService***/
            GenerationBaseService();

            /***生成Service***/
            GenerationService();

            /***修改解决方案***/
            UpdateCsproj();

            /***添加项目引用***/
            Methods.AddRef(IRepositoryProjectName, ModelProjectName);
            Methods.AddRef(RepositoryProjectName, IRepositoryProjectName);
            Methods.AddRef(RepositoryProjectName, ModelProjectName);
            Methods.AddRef(IServiceProjectName, ModelProjectName);
            Methods.AddRef(ServiceNamespace, IRepositoryProjectName);
            Methods.AddRef(ServiceNamespace, IServiceProjectName);
            Methods.AddRef(ServiceNamespace, ModelProjectName);
            Print("引用添加成功");
            Print("项目创建成功");
            Console.ReadLine();
        }

        /// <summary>
        /// 十、生成Service
        /// </summary>
        private static void GenerationService()
        {
            Print("开始创建Service");
            var moduleParameter = new ModuleParameter
            {
                FileSuffix = "Service",
                Parameter = param,
                SavePath = Methods.GetSlnPath + "\\" + ServiceProjectName,
                Tables = db.DbMaintenance.GetTableInfoList().Select(it => it.Name).ToList(),
                TempName = "TempService"
            };
            Methods.CreateModules(moduleParameter);
            AddTask(ServiceProjectName);
            Print("Service创建成功");
        }

        /// <summary>
        /// 九、生成BaseService
        /// </summary>
        private static void GenerationBaseService()
        {
            Print("开始创建BaseService");
            var moduleParameter = new ModuleParameter
            {
                FileName = "BaseService",
                Parameter = param,
                SavePath = Methods.GetSlnPath + "\\" + ServiceProjectName + "\\Base",
                TempName = "TempBaseService"
            };
            Methods.CreateModules(moduleParameter);

            AddTask(ServiceProjectName);
            Print("BaseService创建成功");
        }

        /// <summary>
        /// 八、生成IService
        /// </summary>
        private static void GenerationIService()
        {
            Print("开始创建IService");
            var moduleParameter = new ModuleParameter
            {
                FilePrefix = "I",
                FileSuffix = "Service",
                Parameter = param,
                SavePath = Methods.GetSlnPath + "\\" + IServiceProjectName,
                Tables = db.DbMaintenance.GetTableInfoList().Select(it => it.Name).ToList(),
                TempName = "TempIService"
            };
            Methods.CreateModules(moduleParameter);
            AddTask(IServiceProjectName);
            Print("IService创建成功");
        }

        /// <summary>
        /// 七、生成IBaseService
        /// </summary>
        private static void GenerationIBaseService()
        {
            Print("开始创建IBaseService");
            var moduleParameter = new ModuleParameter
            {
                FileName = "IBaseService",
                Parameter = param,
                SavePath = Methods.GetSlnPath + "\\" + IServiceProjectName + "\\Base",
                TempName = "TempIBaseService"
            };
            Methods.CreateModules(moduleParameter);
            AddTask(IServiceProjectName);
            Print("IBaseService创建成功");
        }

        /// <summary>
        /// 六、生成Repository
        /// </summary>
        private static void GenerationRepository()
        {
            Print("开始创建Repository");
            var moduleParameter = new ModuleParameter
            {
                FileSuffix = "Repository",
                Parameter = param,
                SavePath = Methods.GetSlnPath + "\\" + RepositoryProjectName,
                Tables = db.DbMaintenance.GetTableInfoList().Select(it => it.Name).ToList(),
                TempName = "TempRepository"
            };
            Methods.CreateModules(moduleParameter);

            AddTask(RepositoryProjectName);
            Print("Repository创建成功");
        }

        /// <summary>
        /// 五、生成BaseRepository
        /// </summary>
        private static void GenerationBaseRepository()
        {
            Print("开始创建BaseRepository");
            var moduleParameter = new ModuleParameter
            {
                FileName = "BaseRepository",
                Parameter = param,
                SavePath = Methods.GetSlnPath + "\\" + RepositoryProjectName + "\\Base",
                TempName = "TempBaseRepository"
            };
            Methods.CreateModules(moduleParameter);

            AddTask(RepositoryProjectName);
            Print("BaseRepository创建成功");
        }

        /// <summary>
        /// 四、生成DbContext
        /// </summary>
        private static void GenerationDContext()
        {
            Print("开始创建DbContext");
            param.DbType = dbType;
            param.ConnectionString = connectionString;

            var moduleParameter = new ModuleParameter
            {
                FileName = "DbContext",
                Parameter = param,
                SavePath = Methods.GetSlnPath + "\\" + RepositoryProjectName + "\\Db",
                TempName = "DbContext"
            };
            Methods.CreateModules(moduleParameter);
            AddTask(RepositoryProjectName);
            Print("DbContext创建成功");
        }

        /// <summary>
        /// 三、生成IRepository
        /// </summary>
        private static void GenerationIRepository()
        {
            Print("开始创建IRepository");
            var moduleParameter = new ModuleParameter
            {
                FilePrefix = "I",
                FileSuffix = "Repository",
                Parameter = param,
                SavePath = Methods.GetSlnPath + "\\" + IRepositoryProjectName,
                Tables = db.DbMaintenance.GetTableInfoList().Select(it => it.Name).ToList(),
                TempName = "TempIRepository"
            };
            Methods.CreateModules(moduleParameter);

            AddTask(IRepositoryProjectName);
            Print("IRepository创建成功");
        }

        /// <summary>
        /// 二、生成IBaseRepository
        /// </summary>
        private static void GenerationIBaseRepository()
        {
            Print("开始创建IBaseRepository");
            var moduleParameter = new ModuleParameter
            {
                FileName = "IBaseRepository",
                Parameter = param,
                SavePath = Methods.GetSlnPath + "\\" + IRepositoryProjectName + "\\Base",
                TempName = "TempIBaseRepository"
            };
            Methods.CreateModules(moduleParameter);

            AddTask(IRepositoryProjectName);
            Print("IBaseRepository创建成功");
        }

        /// <summary>
        /// 一、生成实体类
        /// </summary>
        private static void GenerationClass()
        {
            Print("开始创建实体类");
            string classDirectory = Methods.GetSlnPath + "\\" + ModelProjectName;
            db.DbFirst.IsCreateAttribute().CreateClassFile(classDirectory, ModelNamespace);
            AddTask(ModelProjectName);
            Print("实体创建成功");
        }

        #region 辅助方法

        /// <summary>
        ///  修改解决方案
        /// </summary>
        private static void UpdateCsproj()
        {
            foreach (Task item in CsprojList)
            {
                item.Start();
                item.Wait();
            }
        }

        private static void Print(string message)
        {
            Console.WriteLine("");
            Console.WriteLine($"========================={message}=========================");
            Console.WriteLine("");
        }

        private static void AddTask(string bllProjectName)
        {
            Task task = new Task(() =>
            {
                Methods.AddCsproj(bllProjectName);
            });
            CsprojList.Add(task);
        }

        private static List<Task> CsprojList = new List<Task>();

        private static SqlSugar.SqlSugarClient GetDB()
        {
            return new SqlSugar.SqlSugarClient(new SqlSugar.ConnectionConfig()
            {
                DbType = dbType,
                ConnectionString = connectionString,
                IsAutoCloseConnection = true,
                InitKeyType = SqlSugar.InitKeyType.Attribute
            });
        }

        #endregion 辅助方法
    }
}