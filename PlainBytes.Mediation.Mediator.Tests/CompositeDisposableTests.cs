namespace PlainBytes.Mediation.Mediator.Tests
{
    public class CompositeDisposableTests
    {
        [Fact]
        public void Add_When_MultipleDisposablesAdded_Then_CountReflectsNumberOfItems()
        {
            // Arrange
            var composite = new CompositeDisposable();
            var expectedNumberOfItems = 5;
            var items = Enumerable
                .Range(0, expectedNumberOfItems)
                .Select(i => A.Fake<IDisposable>());
            
            // Act
            composite.AddRange(items);

            // Assert
            Assert.Equal(expectedNumberOfItems, composite.Count);
        }

        [Fact]
        public void Add_When_NullDisposable_Then_ThrowsArgumentNullException()
        {
            // Arrange
            var composite = new CompositeDisposable();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => composite.Add(null!));
        }

        [Fact]
        public void Add_When_AlreadyDisposed_Then_DisposesImmediately()
        {
            // Arrange
            var composite = new CompositeDisposable();
            var disposable = A.Fake<IDisposable>();
            composite.Dispose();

            // Act
            composite.Add(disposable);

            // Assert
            Assert.Equal(0, composite.Count);
            A.CallTo(() => disposable.Dispose()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Dispose_When_HasDisposables_Then_DisposesAll()
        {
            // Arrange
            var composite = new CompositeDisposable();
            var expectedNumberOfItems = 5;
            
            var items = Enumerable
                .Range(0, expectedNumberOfItems)
                .Select(i => A.Fake<IDisposable>())
                .ToArray();

            composite.AddRange(items);
            
            // Act
            composite.Dispose();

            // Assert
            Assert.Equal(0, composite.Count);
            foreach (var disposable in items)
            {
                A.CallTo(() => disposable.Dispose()).MustHaveHappenedOnceExactly();
            }
        }

        [Fact]
        public void Dispose_When_Empty_Then_CompletesWithoutError()
        {
            // Arrange
            var composite = new CompositeDisposable();

            // Act
            composite.Dispose();

            // Assert
            Assert.Equal(0, composite.Count);
        }

        [Fact]
        public void Dispose_When_CalledMultipleTimes_Then_OnlyDisposesOnce()
        {
            // Arrange
            var composite = new CompositeDisposable();
            var disposable = A.Fake<IDisposable>();
            composite.Add(disposable);

            // Act
            composite.Dispose();
            composite.Dispose();

            // Assert
            A.CallTo(() => disposable.Dispose()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Dispose_When_OneDisposableThrows_Then_ContinuesDisposingOthers()
        {
            // Arrange
            var composite = new CompositeDisposable();
            var items = Enumerable
                .Range(0, 5)
                .Select(i => A.Fake<IDisposable>())
                .ToArray();

            composite.AddRange(items);

            A.CallTo(() => items[0].Dispose()).Throws<InvalidOperationException>();

            // Act
            composite.Dispose();

            // Assert - all should be called even though disposable2 threw
            foreach (var disposable in items)
            {
                A.CallTo(() => disposable.Dispose()).MustHaveHappenedOnceExactly();
            }
        }

        [Fact]
        public void Count_When_Disposed_Then_ReturnsZero()
        {
            // Arrange
            var composite = new CompositeDisposable();
            var disposable = A.Fake<IDisposable>();
            composite.Add(disposable);

            // Act
            composite.Dispose();

            // Assert
            Assert.Equal(0, composite.Count);
        }

        [Fact]
        public async Task Count_IsThreadSafeAsync()
        {
            // Arrange
            var composite = new CompositeDisposable();
            var tasks = new List<Task>();

            // Act - Add disposables from multiple threads
            for (var i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    var disposable = A.Fake<IDisposable>();
                    composite.Add(disposable);
                }));
            }

            await Task.WhenAll(tasks.ToArray());

            // Assert
            Assert.Equal(100, composite.Count);
        }

        [Fact]
        public async Task Add_When_DisposingConcurrently_Then_HandlesGracefully()
        {
            // Arrange
            var composite = new CompositeDisposable();
            var items = Enumerable
                .Range(0, 3)
                .Select(i => A.Fake<IDisposable>())
                .ToArray();

            composite.AddRange(items.Take(2));
            
            // Act - Dispose while trying to add another
            var addTask = Task.Run(() =>
            {
                Thread.Sleep(10); // Small delay to increase chance of concurrency
                composite.Add(items[2]);
            });

            var disposeTask = Task.Run(() =>
            {
                composite.Dispose();
            });

            await Task.WhenAll(addTask, disposeTask);

            // Assert - Either disposable2 was added before dispose and got disposed,
            // or it was disposed immediately after being added
            foreach (var disposable in items)
            {
                A.CallTo(() => disposable.Dispose()).MustHaveHappenedOnceExactly();
            }
        }
    }
}
