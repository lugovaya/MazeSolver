using MazeSolver.Domain.DataAccess;
using MazeSolver.Domain.Models;

namespace MazeSolver.Tests.DataAccess
{
    [TestFixture]
    public class BasicInMemoryMazeRepositoryFixture
    {
        private BasicInMemoryMazeRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _repository = new BasicInMemoryMazeRepository();
        }

        [Test]
        public void Add_ValidMaze_ShouldAddToRepository()
        {
            var maze = new MazeConfiguration("S________G")
            {
                Id = Guid.NewGuid()
            };

            _repository.Add(maze);
            var retrieved = _repository.Get(maze.Id);

            Assert.That(retrieved.Maze, Is.EqualTo(maze.Maze));
        }

        [Test]
        public void Add_DuplicateMaze_ShouldThrowArgumentException()
        {
            var mazeId = Guid.NewGuid();
            var maze1 = new MazeConfiguration("S________G") { Id = mazeId };
            var maze2 = new MazeConfiguration("S________G") { Id = mazeId };

            _repository.Add(maze1);

            Assert.Throws<ArgumentException>(() => _repository.Add(maze2));
        }

        [Test]
        public void Update_ExistingMaze_ShouldUpdateSuccessfully()
        {
            var mazeId = Guid.NewGuid();
            var maze = new MazeConfiguration("S________G") { Id = mazeId };

            _repository.Add(maze);

            maze.Solution = "Solution";
            _repository.Update(maze);

            var updatedMaze = _repository.Get(mazeId);
            Assert.That(updatedMaze.Solution, Is.EqualTo("Solution"));
        }

        [Test]
        public void Update_NonExistingMaze_ShouldThrowArgumentException()
        {
            var maze = new MazeConfiguration("S________G") { Id = Guid.NewGuid() };

            Assert.Throws<ArgumentException>(() => _repository.Update(maze));
        }

        [Test]
        public void Delete_ExistingMaze_ShouldRemoveSuccessfully()
        {
            var mazeId = Guid.NewGuid();
            var maze = new MazeConfiguration("S________G") { Id = mazeId };

            _repository.Add(maze);
            _repository.Delete(mazeId);

            Assert.Throws<NullReferenceException>(() => _repository.Get(mazeId));
        }

        [Test]
        public void Get_NonExistingMaze_ShouldThrowNullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() => _repository.Get(Guid.NewGuid()));
        }

        [Test]
        public void GetAll_ShouldReturnAllMazes()
        {
            var maze1 = new MazeConfiguration("S________G") { Id = Guid.NewGuid() };
            var maze2 = new MazeConfiguration("S___G") { Id = Guid.NewGuid() };

            _repository.Add(maze1);
            _repository.Add(maze2);

            var allMazes = _repository.GetAll();

            CollectionAssert.AreEquivalent(new[] { maze1, maze2 }, allMazes);
        }
    }
}
