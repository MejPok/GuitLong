using System;
using System.Windows.Threading;

public sealed class PausableTickTimer
{
    private readonly DispatcherTimer _timer = new();

    public event EventHandler? Tick;

    public PausableTickTimer(TimeSpan interval)
    {
        _timer.Interval = interval;
        _timer.Tick += (sender, e) => Tick?.Invoke(this, EventArgs.Empty);
        _timer.Start();
    }

    public void Pause() => _timer.Stop();

    public void Resume() => _timer.Start();

    public void Reset()
    {
        _timer.Stop();
        _timer.Start();
    }

    public void SetInterval(TimeSpan interval)
    {
        _timer.Interval = interval;
        Reset();
    }
}