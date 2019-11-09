using System;
using System.Collections.Generic;
using System.Text;

namespace Generation.Codes
{
    public class ModuleParameter
    {
        /// <summary>
        /// 模板名称
        /// </summary>
        public string TempName { get; set; }
        /// <summary>
        /// 存储目录
        /// </summary>
        public string SavePath { get; set; }
        /// <summary>
        ///  文件名（多文件自动读取表名）
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件前缀
        /// </summary>
        public string FilePrefix { get; set; }
        /// <summary>
        /// 文件后缀
        /// </summary>
        public string FileSuffix { get; set; }
        /// <summary>
        /// 多表集合
        /// </summary>
        public List<string> Tables { get; set; }
        /// <summary>
        /// 模板参数
        /// </summary>
        public TempParameter Parameter { get; set; }
    }
}
