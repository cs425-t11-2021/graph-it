using System;
using System.Threading;

public abstract class Algorithm
{
    protected Graph graph;
	private Thread currThread;
	private Action onThreadExit;


	public Algorithm(Graph graph, Action onThreadExit) {
		this.graph = graph;
        this.currThread = null;
		this.onThreadExit = onThreadExit;
	}

	public abstract void Run();

	public void RunThread() {
		if (currThread != null && currThread.IsAlive) {
			currThread.Abort();
		}

		this.currThread = new Thread(new ThreadStart(RunWrapper));
		currThread.Start();
	}

	private void RunWrapper() {
		try {
			Run();
			RunInMain.singleton.queuedTasks.Enqueue(this.onThreadExit);
		} catch (ThreadAbortException e) {}
	}
}
