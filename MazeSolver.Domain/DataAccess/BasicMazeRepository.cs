using MazeSolver.Domain.Models;
using System.Collections.Concurrent;

namespace MazeSolver.Domain.DataAccess
{
    public class BasicInMemoryMazeRepository : IRepository<MazeConfiguration, Guid>
    {
        // ConcurrentDictionary to ensure thread safety
        private readonly ConcurrentDictionary<Guid, MazeConfiguration> _mazeDataStore = new();

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
            if (!_mazeDataStore.TryRemove(id, out _))
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
