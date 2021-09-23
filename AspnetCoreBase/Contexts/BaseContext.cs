using Microsoft.EntityFrameworkCore;

namespace AspnetCoreBase.Contexts
{
    public class BaseContext : DbContext
    {
        public BaseContext(DbContextOptions options) : base(options) { }
    }
}