using Microsoft.EntityFrameworkCore;

namespace OnlineDemonstrator.EfCli
{
    public interface IContextFactory<out T> where T : DbContext
    {
        T CreateContext();

        T CreateContext(int timeOut);
    }
}
