using System;
using System.Diagnostics;
using System.Windows.Threading;

public sealed class PausableTickTimer
{
    private readonly DispatcherTimer _timer = new();
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private TimeSpan _interval;
    private TimeSpan _nextTickAt;
    private TimeSpan _remaining;
    private bool _paused;

    public event EventHandler? Tick;

    public PausableTickTimer(TimeSpan interval)
    {
        _interval = interval;
        _remaining = interval;
        _timer.Tick += OnTimerTick;
        Schedule(_remaining);
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        _timer.Stop();
        Tick?.Invoke(this, EventArgs.Empty);

        if (!_paused)
        {
            _remaining = _interval;
            Schedule(_remaining);
        }
    }

    private void Schedule(TimeSpan delay)
    {
        _remaining = delay;
        _nextTickAt = _clock.Elapsed + delay;
        _timer.Interval = delay;
        _timer.Start();
    }

    public void Pause()
    {

        if (_paused) return;

        _remaining = _nextTickAt - _clock.Elapsed;

        if (_remaining < TimeSpan.Zero)
            _remaining = TimeSpan.Zero;

        _timer.Stop();
        _paused = true;

    }

    public void Resume()
    {
        if (!_paused) return;

        _paused = false;
        Schedule(_remaining);
    }

    // Start a fresh interval until the next tick.
    public void Reset()
    {
        _timer.Stop();
        _paused = false;
        Schedule(_interval);
    }

    // Optional: change the interval and begin a fresh interval.
    public void SetInterval(TimeSpan interval)
    {
        _interval = interval;
        Reset();
    }
}