using MazeSolver.Domain.Models;

namespace MazeSolver.Domain.Services
{
    public interface IMazeService<TKey, T>
    {
        T Submit(MazeConfigurationBase<T, TKey> mazeConfiguration);
        MazeConfigurationBase<T, TKey> Get(TKey id);
        IEnumerable<MazeConfigurationBase<T, TKey>> GetAll();
    }
}
