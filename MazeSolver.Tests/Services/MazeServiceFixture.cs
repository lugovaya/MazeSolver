using MazeSolver.Domain.DataAccess;
using MazeSolver.Domain.Models;
using MazeSolver.Domain.Services;
using MazeSolver.Domain;
using NSubstitute;

namespace MazeSolver.Tests.Services
{
    [TestFixture]
    public class MazeServiceFixture
    {
        private MazeService _service;
        private IRepository<MazeConfiguration, Guid> _mockRepository;
        private IStringBasedMazeSolver _mockSolver;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = Substitute.For<IRepository<MazeConfiguration, Guid>>();
            _mockSolver = Substitute.For<IStringBasedMazeSolver>();
            _service = new MazeService(_mockRepository, _mockSolver);
        }

        [Test]
        public void Submit_ValidMaze_ShouldSolveAndStore()
        {
            // Arrange
            var mazeId = Guid.NewGuid();
            var maze = new MazeConfiguration("S___G") { Id = mazeId };
            const string expectedSolution = "Solution";

            _mockSolver.Solve(maze.Maze!).Returns(expectedSolution);

            // Act
            var solution = _service.Submit(maze);

            // Assert
            Assert.That(solution, Is.EqualTo(expectedSolution));
            _mockSolver.Received(1).Solve(maze.Maze!);
            _mockRepository
                .Received(1)
                .Add(Arg.Is<MazeConfiguration>(m => m.Maze == maze.Maze && m.Solution == expectedSolution));
        }

        [Test]
        public void Submit_InvalidMaze_ShouldThrowArgumentException()
        {
            // Arrange
            var invalidMaze = new MazeConfiguration("InvalidMaze") { Id = Guid.NewGuid() };

            _mockSolver.Solve(invalidMaze.Maze!).Returns((string?)null);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.Submit(invalidMaze));
            _mockSolver.DidNotReceive().Solve(Arg.Any<string>());
            _mockRepository.DidNotReceive().Add(Arg.Any<MazeConfiguration>());
        }

        [Test]
        public void Get_ExistingMaze_ShouldReturnMaze()
        {
            // Arrange
            var mazeId = Guid.NewGuid();
            var maze = new MazeConfiguration("S___G") { Id = mazeId };

            _mockRepository.Get(mazeId).Returns(maze);

            // Act
            var retrievedMaze = _service.Get(mazeId);

            // Assert
            Assert.That(retrievedMaze, Is.EqualTo(maze));
            _mockRepository.Received(1).Get(mazeId);
        }

        [Test]
        public void Get_NonExistingMaze_ShouldThrowException()
        {
            // Arrange
            var invalidMazeId = Guid.NewGuid();

            _mockRepository.Get(invalidMazeId).Returns(_ => throw new NullReferenceException());

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => _service.Get(invalidMazeId));
            _mockRepository.Received(1).Get(invalidMazeId);
        }

        [Test]
        public void GetAll_ShouldReturnAllMazes()
        {
            // Arrange
            var maze1 = new MazeConfiguration("S___G") { Id = Guid.NewGuid() };
            var maze2 = new MazeConfiguration("S_G") { Id = Guid.NewGuid() };

            var allMazes = new List<MazeConfiguration> { maze1, maze2 };

            _mockRepository.GetAll().Returns(allMazes);

            // Act
            var retrievedMazes = _service.GetAll();

            // Assert
            CollectionAssert.AreEquivalent(allMazes, retrievedMazes);
            _mockRepository.Received(1).GetAll();
        }
    }
}
