using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonSam.Maui.Internal;

internal class InternalTimer : IDisposable
{
    private readonly IDispatcherTimer _timer;
    private readonly Action _callback;
    private bool _isAlive = true;

    public InternalTimer(IDispatcherTimer timer, Action callback)
    {
        _timer = timer;
        _timer.Tick += Timer_Tick;
        _timer.IsRepeating = false;
        _callback = callback;
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {

        _callback();
    }

    public bool IsRunning => _timer.IsRunning;
    public void Start() => _timer.Start();
    public void Stop() => _timer.Stop();

    public void Dispose()
    {
        if (!_isAlive)
            return;

        _isAlive = false;
        _timer.Tick -= Timer_Tick;

        if (IsRunning)
            _timer.Stop();
    }
}