# TP1-DesignPattern
Faire un multithread et un jobserver en c#


## Usage


### Initialisation

```cs
ComS2S com = new ComS2S();
```

On peut initialiser la bibliotheque:

```cs
ComS2S com = new ComS2S(new ComS2S.Options {
    JobSystemOptions = new JobSystem.JobSystemOptions {
        ThreadCount = 4,
    },
    StreamOptions = ComS2S.EStreamOptions.Compressed | ComS2S.EStreamOptions.Encrypted,
});
```

Il faut fermer les connexions a la fin du programme.

```cs
com.Close();
```

### Envoyer des donnees 

```cs
com.SendData(Encoding.UTF8.GetBytes("test"));
```

### Recevoir des donnees 

```cs
static void PrintReceivedData(byte[] data)
{
    Console.WriteLine("Data received: " + Encoding.UTF8.GetString(data));
}

com.OnDataReceived += PrintReceivedData;
```
