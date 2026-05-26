public class CreateNoteCommandHandlerTests
{
    private readonly Mock<INoteRepository> _noteRepositoryMock;
    private readonly Mock<IMessageBus> _messageBusMock;

    private readonly CreateNoteUseCase _useCase;

    public CreateNoteCommandHandlerTests()
    {
        _noteRepositoryMock = new Mock<INoteRepository>();
        _messageBusMock = new Mock<IMessageBus>();

        _useCase = new CreateNoteUseCase(_noteRepositoryMock.Object, _messageBusMock.Object);
    }

    [Fact]
    public async Task Should_Create_Note_When_Content_Validated()
    {
        //arrange
        var content = "This is a test note";
        var userId = Guid.NewGuid();

        //act
        var result = await _useCase.ExecuteAsync(content, userId);

        //assert
        result.Should().NotBeNull();
        _noteRepositoryMock.Verify(x => x.AddAsync(It.Is<Note>(n => n.Content == content && n.UserId == userId)), Times.Once);
        _messageBusMock.Verify(x => x.PublishAsync(It.Is<NoteMessageDto>(m => m.NoteId == result.Id && m.Content == content)), Times.Once);

    }

    [Fact]
    public async Task Should_Reject_Empty_Content()
    {
        // Arrange
        var content = "";
        var userId = Guid.NewGuid();

        // Act
        Func<Task> act = () => _useCase.ExecuteAsync(content, userId);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }
}