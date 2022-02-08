using System;
using System.Threading;

public abstract class Algorithm
{
    protected Graph graph;
	private Thread currThread;
	private Action onThreadExit;
    private bool running;
    private bool completed;
    private int hash;

	public Algorithm(Graph graph, Action onThreadExit) {
		this.graph = graph;
        this.currThread = null;
		this.onThreadExit = onThreadExit;
        this.running = false;
        this.completed = false;
        this.hash = this.GetHashCode();
	}

	public abstract void Run();

	public void RunThread() {
		this.Kill();

		this.currThread = new Thread(new ThreadStart(this.RunWrapper));
		this.currThread.Start();
	}

	private void RunWrapper() {
		try {
            this.running = true;
			this.Run();
            this.running = false;
            this.completed = true;
			RunInMain.singleton.queuedTasks.Enqueue(this.onThreadExit);
		} catch (ThreadAbortException e) { }
	}

    public bool Kill() {
        if (this.currThread != null && this.currThread.IsAlive)
        {
            this.running = false;
            this.currThread.Abort();
            return true;
        }
        return false;
    }
}
