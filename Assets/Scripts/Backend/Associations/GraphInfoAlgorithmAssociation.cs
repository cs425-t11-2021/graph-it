using System;
using System.Linq;

[Serializable]
public class GraphInfoAlgorithmAssociation
{
    public string algorithmClass = "";
    public bool enabled = false;
    public string lead = "";
    public string activationMethod = "";
    public string completedMethod = "";

    public Action OnCompleteUpdateUI
    {
        get
        {
            return () =>
            {
                AlgorithmResult result = (AlgorithmResult) Type.GetType("AlgorithmManager").GetMethod(completedMethod)
                    .Invoke(Controller.Singleton.AlgorithmManager, null);
                
                if (result.type == AlgorithmResultType.ERROR)
                {
                    GraphInfo.Singleton.SetInfoAlgorithmResult(this, lead + ": <color=red>Error</color>");
                    // NotificationManager.Singleton.CreateNotification(this.algorithmClass + "<color=red> Error: " + result.desc + "</color>", 3);
                }
                else if (result.type == AlgorithmResultType.SUCCESS)
                {
                    GraphInfo.Singleton.SetInfoAlgorithmResult( this, lead + ": " + Convert.ToString(Convert.ChangeType(result.results.First().Value.Item1, result.results.First().Value.Item2)));
                }
                else if (result.type == AlgorithmResultType.ESTIMATE)
                {
                    GraphInfo.Singleton.SetInfoAlgorithmResult( this, 
                        "<color=grey><size=-8>" +
                        result.results.Keys.ElementAt(0).ToTitleCase() + ": " + Convert.ToString(Convert.ChangeType(result.results.Values.ElementAt(0).Item1, result.results.Values.ElementAt(0).Item2)) + "\n" +
                        result.results.Keys.ElementAt(1).ToTitleCase() + ": " + Convert.ToString(Convert.ChangeType(result.results.Values.ElementAt(1).Item1, result.results.Values.ElementAt(1).Item2)) + "</size></color>");
                }
            };
        }
    }

    public Action OnEstimateUpdateUI
    {
        get
        {
            return () =>
            {
                AlgorithmResult result = (AlgorithmResult) Type.GetType("AlgorithmManager").GetMethod(completedMethod + "Estimate")
                    .Invoke(Controller.Singleton.AlgorithmManager, null);
                
                if (result.type == AlgorithmResultType.ESTIMATE)
                {
                    GraphInfo.Singleton.SetInfoAlgorithmResult( this, 
                        "<color=grey>" +
                        result.results.Keys.ElementAt(0).ToTitleCase() + ": " + Convert.ToString(Convert.ChangeType(result.results.Values.ElementAt(0).Item1, result.results.Values.ElementAt(0).Item2)) + "\n" +
                        result.results.Keys.ElementAt(1).ToTitleCase() + ": " + Convert.ToString(Convert.ChangeType(result.results.Values.ElementAt(1).Item1, result.results.Values.ElementAt(1).Item2)) + "</color>");
                }
            };
        }
    }

    public Action OnCalculatingUpdateUI
    {
        get
        {
            return () =>
            {
                GraphInfo.Singleton.SetInfoAlgorithmResult(this, lead + ": ...");
            };
        }
    }
}