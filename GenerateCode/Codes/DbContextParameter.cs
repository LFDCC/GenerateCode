using SqlSugar;

namespace Generation
{
    public class DbContextParameter
    {
        public string ConnectionString { get; set; }
        public DbType DbType { get; set; }
        public string Namespace { get; set; }
    }
}