using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using RazorEngine;
using RazorEngine.Templating;

namespace Generation.Codes
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
                string path1 = Directory.GetCurrentDirectory();
                return @"C:\Testing";
            }
        }

        /// <summary>
        /// 添加引用
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="refProjectName"></param>
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

        /// <summary>
        /// 创建模块
        /// </summary>
        public static void CreateModules(ModuleParameter moduleParameter)
        {
            string template = System.IO.File.ReadAllText($"{Methods.GetCurrentProjectPath}\\Template\\{moduleParameter.TempName}.txt"); //从文件中读出模板内容
            string templateKey = Guid.NewGuid().ToString("N"); //取个名字
            if (moduleParameter.Tables?.Count > 0)
            {
                foreach (string item in moduleParameter.Tables)
                {
                    moduleParameter.Parameter.TableName = item;
                    string result = Engine.Razor.RunCompile(template, templateKey, moduleParameter.Parameter.GetType(), moduleParameter.Parameter);
                    string savePath = $"{moduleParameter.SavePath}\\{moduleParameter.FilePrefix}{item}{moduleParameter.FileSuffix}.cs";
                    if (FileHelper.IsExistFile(savePath) == false)
                    {
                        FileHelper.CreateFile(savePath, result, System.Text.Encoding.UTF8);
                    }
                }
            }
            else
            {
                string result = Engine.Razor.RunCompile(template, templateKey, moduleParameter.Parameter.GetType(), moduleParameter.Parameter);
                string savePath = $"{moduleParameter.SavePath}\\{moduleParameter.FilePrefix}{moduleParameter.FileName}{moduleParameter.FileSuffix}.cs";
                if (FileHelper.IsExistFile(savePath) == false)
                {
                    FileHelper.CreateFile(savePath, result, System.Text.Encoding.UTF8);
                }
            }
        }

        /// <summary>
        /// 创建项目
        /// </summary>
        /// <param name="projectName"></param>
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