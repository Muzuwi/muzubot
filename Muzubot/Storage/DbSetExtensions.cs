using Microsoft.EntityFrameworkCore;

namespace Muzubot.Storage;

public static class DbSetExtensions
{
    public static void AddOrUpdate<T>(this DbSet<T> set, T entity) where T : class
    {
        if (set.Entry(entity).State == EntityState.Detached)
        {
            set.Add(entity);
        }
        else if (set.Entry(entity).State == EntityState.Modified)
        {
            set.Update(entity);
        }
        else
        {
            throw new NotImplementedException("unreachable");
        }
    }
}