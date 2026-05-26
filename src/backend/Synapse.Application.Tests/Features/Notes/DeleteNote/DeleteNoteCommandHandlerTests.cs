public class DeleteNoteCommandHandlerTests
{
    private readonly Mock<INoteRepository> _noteRepositoryMock;

    private readonly DeleteNoteUseCase _useCase;

    public DeleteNoteCommandHandlerTests()
    {
        _noteRepositoryMock = new Mock<INoteRepository>();

        _useCase = new DeleteNoteUseCase(_noteRepositoryMock.Object);
    }

    [Fact]
    public async Task Should_Delete_Note_When_Note_Exists()
    {
        //arrange
        var noteId = Guid.NewGuid();
        var note = new Note
        {
            Id = noteId,
            Content = "Test note",
            UserId = Guid.NewGuid(),
            Status = NoteStatus.Completed
        };

        _noteRepositoryMock.Setup(x => x.GetByIdAsync(noteId)).ReturnsAsync(note);

        var userId = note.UserId;

        //act
        var result = await _useCase.ExecuteAsync(noteId, userId);

        //assert
        result.Should().BeTrue();
        _noteRepositoryMock.Verify(x => x.DeleteAsync(noteId), Times.Once);

    }

    [Fact]
    public async Task Should_Reject_Invalid_NoteId()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _noteRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((Note)null);

        // Act
        Func<Task> act = () => _useCase.ExecuteAsync(id, userId);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }
    
    [Fact]
    public async Task Should_Reject_None_Owner_Request()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var note = new Note
        {
            Id = id,
            Content = "Test note",
            UserId = Guid.NewGuid(), // different user
            Status = NoteStatus.Completed
        };

        // Act
        Func<Task> act = () => _useCase.ExecuteAsync(id, userId);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }
}