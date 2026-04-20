# Pong

## Lecture where we create the famous early video game "Pong"
## https://en.wikipedia.org/wiki/Pong


## Example of what Console.Clear() does
```cs
string spam = "HelloHelloWorldWorldHello";

        foreach (var s in spam)
        {
            Console.WriteLine(s);
        }
        Console.Clear();
```


### Enum example
```cs
public enum KeyboardKeys 
{
    Enter = 1,
    Space = 2,
    Alt = 3,
    Tab = 4,
    Shift = 5, 
}
```

## Match string[] args to console keys
```cs
private void ValidateInputKeys(ConsoleKey key)
{
    args[1] = key.E;
    args[2] = key.L;
}

switch(key) 
{
    case ConsoleKey.E:
        args[1] = EchoImplementation.Echo(args[2]);
        break;
}
```

# When we run a .NET program in the console, here is how string[] args works

```sh
dotnet run # this has no arguments so args[0] == string.Empty

# we use the string[] args to get string arguments in the console application
dotnet run "hello, world" # in this instance, args[0] == "hello, world"
```