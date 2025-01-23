using MazeSolver.Domain.Models;

namespace MazeSolver.Domain.DataAccess
{
    public class BasicInMemoryMazeRepository : IRepository<MazeConfiguration, Guid>
    {
        // TODO: lazy initialization with static field to ensure thread safety in singleton 

        // Dictionary to simulate in-memory database, key is the maze Id
        private readonly Dictionary<Guid, MazeConfiguration> _mazeDataStore = [];

        public void Add(MazeConfiguration entity)
        {
            if (_mazeDataStore.ContainsKey(entity.Id))
            {
                throw new ArgumentException("Maze with the same Id already exists.");
            }

            // Add the new maze to the dictionary
            _mazeDataStore[entity.Id] = entity;
        }

        public void Delete(Guid id)
        {
            // Remove the maze from the dictionary if it exists
            if (!_mazeDataStore.Remove(id))
            {
                throw new ArgumentException("Maze with the specified Id not found.");
            }
        }

        public MazeConfiguration Get(Guid id)
        {
            // Check if the maze exists in the dictionary
            if (_mazeDataStore.TryGetValue(id, out var maze))
            {
                return maze;
            }
            throw new NullReferenceException($"Could not find the maze with ID {id}");
        }

        public IEnumerable<MazeConfiguration> GetAll()
        {
            return _mazeDataStore.Values;
        }

        public void Update(MazeConfiguration entity)
        {
            if (_mazeDataStore.ContainsKey(entity.Id))
            {
                // Update the existing maze configuration
                _mazeDataStore[entity.Id] = entity;
            }
            else
            {
                throw new ArgumentException($"Could not find the maze with ID {entity.Id}");
            }
        }
    }
}
