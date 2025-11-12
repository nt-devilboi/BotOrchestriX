using BotOrchestriX.Entity;
using Newtonsoft.Json;
using Stateless;

namespace BotOrchestriX.Abstract;

public class DetailContext<TPayload, TState>
    where TState : struct, Enum
    where TPayload : BasePayload
{
    private TPayload? _payload;

    private readonly ChatContext _chatContext;

    internal DetailContext(ChatContext chatContext, IServiceRegistryFlow registryFlow)
    {
        var stateMachine = new StateMachine<TState, Trigger>(() => Enum.Parse<TState>(chatContext.State),
            x => chatContext.State = x.ToString());

        State = registryFlow.Wraps(stateMachine);
        _chatContext = chatContext;
        _payload = JsonConvert.DeserializeObject<TPayload>(_chatContext.Payload ?? string.Empty);
    }

    public IStateMachine<TState> State { get; }
    public long ChatId => _chatContext.ChatId;

    public bool TryGetPayload(out TPayload payload)
    {
        if (_payload != null)
        {
            payload = _payload;
            return true;
        }

        payload = null;
        return false;
    }


    public DetailContext<TPayload, TState> Reset()
    {
        _chatContext.ToMenu();
        _chatContext.Payload = null;
        return this;
    }

    public DetailContext<TPayload, TState> UpdatePayload(TPayload payload)
    {
        _payload = payload;
        _chatContext.Payload = JsonConvert.SerializeObject(_payload);

        return this;
    }
}

public abstract record BasePayload;