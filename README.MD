# ShowCase
https://raw.githubusercontent.com/Vlvin/DiceGame/main/ShowCase.mp4


#" Installation
```sh
git clone --depth=1 git@github.com:Vlvin/DiceGame
```



# Run
## Dependencies
  - .NET

## First launch
```sh
cd DiceGame
dotnet run <dice configuration>
```
## Second launch 
```sh
cd DiceGame/bin/Debug/net<.NET version>/
dotnet DiceGame.dll <dice configuration>
```

## Dice Configuration
You should pass at least 2 space-separated dice as CLI args.
Die could be any size
for example 6d in args looks like this
```
1,2,3,4,5,6
```

