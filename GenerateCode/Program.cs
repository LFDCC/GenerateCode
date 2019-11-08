using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlSugar;
using SugarCodeGeneration.Codes;

namespace SugarCodeGeneration
{
    /// <summary>
    /// F5直接运行生成项目
    /// </summary>
    /// <param name="args"></param>
    internal class Program
    {
        //如果你不需要自定义，直接配好数据库连接，F5运行项目
        private const SqlSugar.DbType dbType = SqlSugar.DbType.SqlServer;

        /// <summary>
        /// 连接字符串
        /// </summary>
        private const string connectionString = @"server=.\sqlexpress;uid=sa;pwd=ok;database=lyq";

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

        private const string IBaseRepositoryNamespace = IRepositoryNamespace + ".Base";
        private const string BaseRepositoryNamespace = RepositoryNamespace + ".Base";

        private static SqlSugarClient db;

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
            string savePath = Methods.GetSlnPath + "\\" + ServiceProjectName;//保存目录
            List<string> tables = db.DbMaintenance.GetTableInfoList().Select(it => it.Name).ToList();
            string templatePath = Methods.GetCurrentProjectPath + "\\Template\\TempService.txt";//模版地址

            Methods.CreateService(templatePath, savePath, tables, ServiceNamespace, ModelNamespace, IServiceNamespace, ServiceNamespace + ".Base", IRepositoryNamespace);
            AddTask(ServiceProjectName);
            Print("Service创建成功");
        }

        /// <summary>
        /// 九、生成BaseService
        /// </summary>
        private static void GenerationBaseService()
        {
            Print("开始创建BaseService");
            string templatePath = Methods.GetCurrentProjectPath + "\\Template\\TempBaseService.txt";//模版地址
            string savePath = Methods.GetSlnPath + "\\" + SolutionName + ".Service" + "\\Base\\BaseService.cs";//具体文件名

            Methods.CreateBaseService(templatePath, savePath, ServiceNamespace + ".Base", IServiceNamespace, IRepositoryNamespace);
            AddTask(ServiceProjectName);
            Print("BaseService创建成功");
        }

        /// <summary>
        /// 八、生成IService
        /// </summary>
        private static void GenerationIService()
        {
            Print("开始创建IService");
            string savePath = Methods.GetSlnPath + "\\" + IServiceProjectName;//保存目录
            List<string> tables = db.DbMaintenance.GetTableInfoList().Select(it => it.Name).ToList();
            string templatePath = Methods.GetCurrentProjectPath + "\\Template\\TempIService.txt";//模版地址

            Methods.CreateIService(templatePath, savePath, tables, IServiceNamespace, ModelNamespace, IServiceNamespace + ".Base");
            AddTask(IServiceProjectName);
            Print("IService创建成功");
        }

        /// <summary>
        /// 七、生成IBaseService
        /// </summary>
        private static void GenerationIBaseService()
        {
            Print("开始创建IBaseService");
            string templatePath = Methods.GetCurrentProjectPath + "\\Template\\TempIBaseService.txt";//模版地址
            string savePath = Methods.GetSlnPath + "\\" + SolutionName + ".IService" + "\\Base\\IBaseService.cs";//具体文件名

            Methods.CreateIBaseService(templatePath, savePath, IServiceNamespace + ".Base");
            AddTask(IServiceProjectName);
            Print("IBaseService创建成功");
        }

        /// <summary>
        /// 六、生成Repository
        /// </summary>
        private static void GenerationRepository()
        {
            Print("开始创建Repository");
            string savePath = Methods.GetSlnPath + "\\" + RepositoryProjectName;//保存目录
            List<string> tables = db.DbMaintenance.GetTableInfoList().Select(it => it.Name).ToList();
            string templatePath = Methods.GetCurrentProjectPath + "\\Template\\TempRepository.txt";//模版地址

            Methods.CreateRepository(templatePath, savePath, tables, RepositoryNamespace, ModelNamespace, IRepositoryNamespace, RepositoryNamespace + ".Base");
            AddTask(RepositoryProjectName);
            Print("Repository创建成功");
        }

        /// <summary>
        /// 五、生成BaseRepository
        /// </summary>
        private static void GenerationBaseRepository()
        {
            Print("开始创建BaseRepository");
            string templatePath = Methods.GetCurrentProjectPath + "\\Template\\TempBaseRepository.txt";//模版地址
            string savePath = Methods.GetSlnPath + "\\" + SolutionName + ".Repository" + "\\Base\\BaseRepository.cs";//具体文件名

            Methods.CreateBaseRepository(templatePath, savePath, RepositoryNamespace + ".Base", ModelNamespace, IRepositoryNamespace);
            AddTask(RepositoryProjectName);
            Print("BaseRepository创建成功");
        }

        /// <summary>
        /// 四、生成DbContext
        /// </summary>
        private static void GenerationDContext()
        {
            Print("开始创建DbContext");
            string templatePath = Methods.GetCurrentProjectPath + "\\Template\\DbContext.txt";//模版地址
            string savePath = Methods.GetSlnPath + "\\" + RepositoryProjectName + "\\Db\\DbContext.cs";//具体文件名
            //下面代码不动
            DbContextParameter model = new DbContextParameter
            {
                ConnectionString = connectionString,
                DbType = dbType,
                Namespace = RepositoryNamespace + ".Db"
            };
            Methods.CreateDbContext(templatePath, savePath, model);
            AddTask(RepositoryProjectName);
            Print("DbContext创建成功");
        }

        /// <summary>
        /// 三、生成IRepository
        /// </summary>
        private static void GenerationIRepository()
        {
            Print("开始创建IRepository");
            string savePath = Methods.GetSlnPath + "\\" + IRepositoryProjectName;//保存目录
            List<string> tables = db.DbMaintenance.GetTableInfoList().Select(it => it.Name).ToList();
            string templatePath = Methods.GetCurrentProjectPath + "\\Template\\TempIRepository.txt";//模版地址

            Methods.CreateIRepository(templatePath, savePath, tables, IRepositoryNamespace, ModelNamespace, IRepositoryNamespace + ".Base");
            AddTask(IRepositoryProjectName);
            Print("IRepository创建成功");
        }

        /// <summary>
        /// 二、生成IBaseRepository
        /// </summary>
        private static void GenerationIBaseRepository()
        {
            Print("开始创建IBaseRepository");
            string templatePath = Methods.GetCurrentProjectPath + "\\Template\\TempIBaseRepository.txt";//模版地址
            string savePath = Methods.GetSlnPath + "\\" + SolutionName + ".IRepository" + "\\Base\\IBaseRepository.cs";//具体文件名

            Methods.CreateIBaseRepository(templatePath, savePath, IRepositoryNamespace + ".Base");
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