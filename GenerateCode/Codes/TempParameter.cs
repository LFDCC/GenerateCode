
using SqlSugar;

namespace Generation
{
    public class TempParameter
    {
        public string ConnectionString { get; set; }
        public DbType DbType { get; set; }
        public string TableName { get; set; }
        public string ModelNamespace { get; set; }
        public string IRepositoryNamespace { get; set; }
        public string IBaseRepositoryNamespace => IRepositoryNamespace + ".Base";
        public string RepositoryNamespace { get; set; }
        public string BaseRepositoryNamespace => RepositoryNamespace + ".Base";
        public string DbContextNamespace => RepositoryNamespace + ".Db";
        public string IServiceNamespace { get; set; }
        public string IBaseServiceNamespace => IServiceNamespace + ".Base";
        public string ServiceNamespace { get; set; }
        public string BaseServiceNamespace => ServiceNamespace + ".Base";
    }
}