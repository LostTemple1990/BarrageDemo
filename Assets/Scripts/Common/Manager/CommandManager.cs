using System.Collections.Generic;

public class CommandManager
{
    private static CommandManager _instance = new CommandManager();

    public static CommandManager GetInstance()
    {
        return _instance;
    }

    private Dictionary<int, List<ICommand>> _commandsMap;

    public void Init()
    {
        if ( _commandsMap == null )
        {
            _commandsMap = new Dictionary<int, List<ICommand>>();
        }
    }

    public void Register(int cmd,ICommand command)
    {
        List<ICommand> _commandList;
        if ( !_commandsMap.TryGetValue(cmd,out _commandList) )
        {
            _commandList = new List<ICommand>();
            _commandsMap.Add(cmd, _commandList);
        }
        if ( !_commandList.Contains(command) )
        {
            _commandList.Add(command);
        }
    }

    public void Remove(int cmd,ICommand command)
    {
        List<ICommand> commandList;
        if (_commandsMap.TryGetValue(cmd, out commandList))
        {
            int i;
            int count = commandList.Count;
            for (i = 0; i < count; i++)
            {
                if (commandList[i] == command)
                {
                    commandList[i] = null;
                }
            }
        }
    }

    public void RunCommand(int cmd,object[] data)
    {
        List<ICommand> commandList;
        if (_commandsMap.TryGetValue(cmd, out commandList))
        {
            int i;
            int count = commandList.Count;
            int removeFlag = 0;
            for (i = 0; i < count; i++)
            {
                if (commandList[i] != null )
                {
                    commandList[i].Execute(cmd, data);
                }
                else
                {
                    removeFlag = 1;
                }
            }
            if (removeFlag == 1 )
            {
                CommonUtils.RemoveNullElementsInList<ICommand>(commandList);
            }
        }
    }
}

public interface ICommand
{
    void Execute(int cmd, object[] data);
}