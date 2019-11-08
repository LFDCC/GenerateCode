using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using RazorEngine;
using RazorEngine.Templating;

namespace SugarCodeGeneration.Codes
{
    /// <summary>
    /// 生成所需要的代码
    /// </summary>
    public class Methods
    {
        public static Dictionary<string, string> ProjectIds = new Dictionary<string, string>();

        public static string GetCurrentProjectPath => Environment.CurrentDirectory.Replace(@"\bin\Debug", "").Replace("\\netcoreapp2.0", "");

        public static string GetSlnPath
        {
            get
            {
                string path = Directory.GetParent(GetCurrentProjectPath).FullName;
                return @"C:\Testing";
            }
        }

        public static void AddRef(string projectName, string refProjectName)
        {
            string xmlPath = GetSlnPath + @"\" + projectName + @"\" + projectName + ".csproj";

            string xml = File.ReadAllText(xmlPath, System.Text.Encoding.UTF8);
            if (xml.Contains(refProjectName))
            {
                return;
            }

            string firstLine = System.IO.File.ReadLines(xmlPath, System.Text.Encoding.UTF8).First();
            string newXml = xml.Replace(firstLine, "").TrimStart('\r').TrimStart('\n');
            XDocument xe = XDocument.Load(xmlPath);
            XElement root = xe.Root;

            XElement itemGroup = new XElement("ItemGroup");
            XElement refItem = new XElement("ProjectReference", new XAttribute("Include", string.Format(@"..\{0}\{0}.csproj", refProjectName)));
            refItem.Add(new XElement("Name", refProjectName));
            refItem.Add(new XElement("Project", "{" + ProjectIds[refProjectName] + "}"));
            itemGroup.Add(refItem);
            root.Add(itemGroup);

            newXml = xe.ToString().Replace("xmlns=\"\"", "");
            xe = XDocument.Parse(newXml);
            xe.Save(xmlPath);
        }

        public static void AddCsproj(string projectName)
        {
            CreateProject(projectName);
            string xmlPath = GetSlnPath + @"\" + projectName + @"\" + projectName + ".csproj";

            string xml = File.ReadAllText(xmlPath, System.Text.Encoding.UTF8);
            string firstLine = System.IO.File.ReadLines(xmlPath, System.Text.Encoding.UTF8).First();
            string newXml = xml.Replace(firstLine, "").TrimStart('\r').TrimStart('\n');
            XDocument xe = XDocument.Load(xmlPath);
            newXml = xe.ToString().Replace("xmlns=\"\"", "");
            xe = XDocument.Parse(newXml);
            xe.Save(xmlPath);
        }

        /// <summary>
        /// 生成IBaseService
        /// </summary>
        /// <param name="templatePath"></param>
        /// <param name="savePath"></param>
        /// <param name="nameSpace"></param>
        public static void CreateIBaseService(string templatePath, string savePath, string nameSpace)
        {
            string template = System.IO.File.ReadAllText(templatePath); //从文件中读出模板内容
            string templateKey = "ibs"; //取个名字
            Parameter model = new Parameter()
            {
                Namespace = nameSpace
            };
            string result = Engine.Razor.RunCompile(template, templateKey, model.GetType(), model);
            if (FileHelper.IsExistFile(savePath) == false)
            {
                FileHelper.CreateFile(savePath, result, System.Text.Encoding.UTF8);
            }
        }

        /// <summary>
        /// 生成IService
        /// </summary>
        /// <param name="templatePath"></param>
        /// <param name="savePath"></param>
        /// <param name="tables"></param>
        /// <param name="nameSpace"></param>
        /// <param name="modelNamespace"></param>
        /// <param name="iBaseRepositoryNamespace"></param>
        public static void CreateIService(string templatePath, string savePath, List<string> tables, string nameSpace, string modelNamespace, string iBaseServiceNamespace)
        {
            string template = System.IO.File.ReadAllText(templatePath); //从文件中读出模板内容
            string templateKey = "is"; //取个名字
            foreach (string item in tables)
            {
                Parameter model = new Parameter()
                {
                    Name = item,
                    Namespace = nameSpace,
                    ModelNamespace = modelNamespace,
                    IServiceNamespace = iBaseServiceNamespace
                };
                string result = Engine.Razor.RunCompile(template, templateKey, model.GetType(), model);
                string cp = savePath + "\\I" + item + "Service.cs";
                if (FileHelper.IsExistFile(cp) == false)
                {
                    FileHelper.CreateFile(cp, result, System.Text.Encoding.UTF8);
                }
            }
        }

        /// <summary>
        /// 生成dbcontext
        /// </summary>
        /// <param name="templatePath"></param>
        /// <param name="savePath"></param>
        /// <param name="model"></param>
        public static void CreateDbContext(string templatePath, string savePath, DbContextParameter model)
        {
            string template = System.IO.File.ReadAllText(templatePath); //从文件中读出模板内容
            string templateKey = "dbcontext"; //取个名字
            string result = Engine.Razor.RunCompile(template, templateKey, model.GetType(), model);
            if (FileHelper.IsExistFile(savePath) == false)
            {
                FileHelper.CreateFile(savePath, result, System.Text.Encoding.UTF8);
            }
        }

        public static void CreateBaseService(string templatePath, string savePath, string nameSpace, string iServiceNamespace, string iRepositoryNamespace)
        {
            string template = System.IO.File.ReadAllText(templatePath); //从文件中读出模板内容
            string templateKey = "bs"; //取个名字
            Parameter model = new Parameter()
            {
                Namespace = nameSpace,
                IServiceNamespace = iServiceNamespace + ".Base",
                IRepositoryNamespace = iRepositoryNamespace + ".Base"
            };
            string result = Engine.Razor.RunCompile(template, templateKey, model.GetType(), model);

            if (FileHelper.IsExistFile(savePath) == false)
            {
                FileHelper.CreateFile(savePath, result, System.Text.Encoding.UTF8);
            }
        }

        public static void CreateService(string templatePath, string savePath, List<string> tables, string nameSpace, string modelNamespace, string iServiceNamespace, string baseServiceNamespace, string iRepositoryNamespace)
        {
            string template = System.IO.File.ReadAllText(templatePath); //从文件中读出模板内容
            string templateKey = "sss"; //取个名字
            foreach (string item in tables)
            {
                Parameter model = new Parameter()
                {
                    Name = item,
                    Namespace = nameSpace,
                    ModelNamespace = modelNamespace,
                    IServiceNamespace = iServiceNamespace,
                    ServiceNamespace = baseServiceNamespace,
                    IRepositoryNamespace = iRepositoryNamespace
                };
                string result = Engine.Razor.RunCompile(template, templateKey, model.GetType(), model);
                string cp = savePath + "\\" + item + "Service.cs";
                if (FileHelper.IsExistFile(cp) == false)
                {
                    FileHelper.CreateFile(cp, result, System.Text.Encoding.UTF8);
                }
            }
        }

        public static void CreateRepository(string templatePath, string savePath, List<string> tables, string nameSpace, string modelNamespace, string iRepositoryNamespace, string baseRepositoryNamespace)
        {
            string template = System.IO.File.ReadAllText(templatePath); //从文件中读出模板内容
            string templateKey = "cr"; //取个名字
            foreach (string item in tables)
            {
                Parameter model = new Parameter()
                {
                    Name = item,
                    Namespace = nameSpace,
                    IRepositoryNamespace = iRepositoryNamespace,
                    ModelNamespace = modelNamespace,
                    RepositoryNamespace = baseRepositoryNamespace
                };
                string result = Engine.Razor.RunCompile(template, templateKey, model.GetType(), model);
                string cp = savePath + "\\" + item + "Repository.cs";
                if (FileHelper.IsExistFile(cp) == false)
                {
                    FileHelper.CreateFile(cp, result, System.Text.Encoding.UTF8);
                }
            }
        }

        public static void CreateBaseRepository(string templatePath, string savePath, string nameSpace, string modelNamespace, string iRepositoryNamespace)
        {
            string template = System.IO.File.ReadAllText(templatePath); //从文件中读出模板内容
            string templateKey = "br"; //取个名字
            Parameter model = new Parameter()
            {
                ModelNamespace = modelNamespace,
                IRepositoryNamespace = iRepositoryNamespace + ".Base",
                Namespace = nameSpace
            };
            string result = Engine.Razor.RunCompile(template, templateKey, model.GetType(), model);

            if (FileHelper.IsExistFile(savePath) == false)
            {
                FileHelper.CreateFile(savePath, result, System.Text.Encoding.UTF8);
            }
        }

        /// <summary>
        /// 生成IBaseRepository
        /// </summary>
        /// <param name="templatePath"></param>
        /// <param name="savePath"></param>
        /// <param name="nameSpace"></param>
        public static void CreateIBaseRepository(string templatePath, string savePath, string nameSpace)
        {
            string template = System.IO.File.ReadAllText(templatePath); //从文件中读出模板内容
            string templateKey = "ibr"; //取个名字
            Parameter model = new Parameter()
            {
                Namespace = nameSpace
            };
            string result = Engine.Razor.RunCompile(template, templateKey, model.GetType(), model);
            if (FileHelper.IsExistFile(savePath) == false)
            {
                FileHelper.CreateFile(savePath, result, System.Text.Encoding.UTF8);
            }
        }

        /// <summary>
        /// 创建仓储接口
        /// </summary>
        /// <param name="templatePath"></param>
        /// <param name="savePath"></param>
        /// <param name="tables"></param>
        /// <param name="nameSpace"></param>
        /// <param name="modelNamespace"></param>
        /// <param name="iBaseRepositoryNamespace"></param>
        public static void CreateIRepository(string templatePath, string savePath, List<string> tables, string nameSpace, string modelNamespace, string iBaseRepositoryNamespace)
        {
            string template = System.IO.File.ReadAllText(templatePath); //从文件中读出模板内容
            string templateKey = "ir"; //取个名字
            foreach (string item in tables)
            {
                Parameter model = new Parameter()
                {
                    Name = item,
                    Namespace = nameSpace,
                    ModelNamespace = modelNamespace,
                    IRepositoryNamespace = iBaseRepositoryNamespace
                };
                string result = Engine.Razor.RunCompile(template, templateKey, model.GetType(), model);
                string cp = savePath + "\\I" + item + "Repository.cs";
                if (FileHelper.IsExistFile(cp) == false)
                {
                    FileHelper.CreateFile(cp, result, System.Text.Encoding.UTF8);
                }
            }
        }

        /// <summary>
        /// 创建项目
        /// </summary>
        /// <param name="name"></param>
        public static void CreateProject(string name)
        {
            string templatePath = GetCurrentProjectPath + "/Template/Project.txt";
            string projectId = Guid.NewGuid().ToString();
            string project = System.IO.File.ReadAllText(templatePath).Replace("@pid", projectId).Replace("@AssemblyName", name); //从文件中读出模板内容
            string projectPath = GetSlnPath + "\\" + name + "\\" + name + ".csproj";
            string projectDic = GetSlnPath + "\\" + name + "\\";
            string binDic = GetSlnPath + "\\" + name + "\\bin";
            if (!FileHelper.IsExistFile(projectPath))
            {
                if (!FileHelper.IsExistDirectory(projectDic))
                {
                    FileHelper.CreateDirectory(projectDic);
                }
                if (!FileHelper.IsExistDirectory(binDic))
                {
                    FileHelper.CreateDirectory(binDic);
                }
                FileHelper.CreateFile(projectPath, project, System.Text.Encoding.UTF8);
                //FileHelper.CreateFile(projectDic + "\\class1.cs", "", System.Text.Encoding.UTF8);
                //File.Copy(GetCurrentProjectPath + "/Template/nuget.txt", projectDic + "packages.config");
                ProjectIds.Add(name, projectId);
                AppendProjectToSln(projectId, name);
            }
        }

        /// <summary>
        /// 将项目添加至解决方案
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="projectName"></param>
        public static void AppendProjectToSln(string projectId, string projectName)
        {
            IEnumerable<string> slns = Directory.GetFiles(GetSlnPath).Where(it => it.Contains(".sln"));
            if (slns.Any())
            {
                string sln = slns.First();
                string templatePath = GetCurrentProjectPath + "/Template/sln.txt";
                string appendText = System.IO.File.ReadAllText(templatePath)
                    .Replace("@pid", projectId)
                    .Replace("@name", projectName)
                    .Replace("@sid", Guid.NewGuid().ToString());
                FileStream fs = new FileStream(sln, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(appendText);
                sw.Close();
                fs.Close();
            }
        }

        /// <summary>
        /// 创建解决方案
        /// </summary>
        /// <param name="name"></param>
        public static void CreateSln(string name)
        {
            if (!FileHelper.IsExistDirectory(GetSlnPath))
            {
                FileHelper.CreateDirectory(GetSlnPath);
            }

            string templatePath = GetCurrentProjectPath + "/Template/SlnMain.txt";
            string projectId = Guid.NewGuid().ToString();
            string project = System.IO.File.ReadAllText(templatePath); //从文件中读出模板内容
            string projectPath = GetSlnPath + "\\" + name + ".sln";
            if (!FileHelper.IsExistFile(projectPath))
            {
                FileHelper.CreateFile(projectPath, project, System.Text.Encoding.UTF8);
            }
        }
    }
}