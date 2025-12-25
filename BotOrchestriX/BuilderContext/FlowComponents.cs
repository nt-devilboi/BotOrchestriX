namespace BotOrchestriX;

public class FlowComponents
{
    private readonly List<string> States = [];
    public string Start => States[0];

    public void Add(string state)
    {
        States.Add(state);
    }
}