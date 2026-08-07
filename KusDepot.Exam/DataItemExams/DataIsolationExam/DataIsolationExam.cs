namespace KusDepot.Exams.DataItems;

[TestFixture]
[NonParallelizable]
public class DataIsolationExam
{
    [Test]
    public async Task BeginScope_DoesNotAffectConcurrentWork()
    {
        DataIsolation.Enable();

        Task<Boolean> disabledScope = Task.Run(async () =>
        {
            using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
            {
                await Task.Delay(10).ConfigureAwait(false);

                return DataIsolation.IsEnabled();
            }
        });

        Task<Boolean> enabledScope = Task.Run(async () =>
        {
            using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
            {
                await Task.Delay(10).ConfigureAwait(false);

                return DataIsolation.IsEnabled();
            }
        });

        Boolean[] results = await Task.WhenAll(disabledScope,enabledScope).ConfigureAwait(false);

        Check.That(results[0]).IsFalse();
        Check.That(results[1]).IsTrue();
        Check.That(DataIsolation.IsEnabled()).IsTrue();
    }

    [Test]
    public void GlobalMode_CanVary_IndependentlyOfScopedOverride()
    {
        DataIsolation.Enable();

        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            Check.That(DataIsolation.IsEnabled()).IsFalse();

            DataIsolation.Enable();
            Check.That(DataIsolation.IsEnabled()).IsFalse();

            DataIsolation.Disable();
            Check.That(DataIsolation.IsEnabled()).IsFalse();
        }

        Check.That(DataIsolation.IsDisabled()).IsTrue();

        DataIsolation.Enable();
        Check.That(DataIsolation.IsEnabled()).IsTrue();
    }

    [Test]
    public void IsScopeActive_ReflectsCurrentScopeState()
    {
        Check.That(DataIsolation.IsScopeActive()).IsFalse();

        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            Check.That(DataIsolation.IsScopeActive()).IsTrue();
        }

        Check.That(DataIsolation.IsScopeActive()).IsFalse();
    }
}
