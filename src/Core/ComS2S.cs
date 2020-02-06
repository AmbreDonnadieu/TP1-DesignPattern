using System;
using System.Text;
using System.Threading;
using JobSystem;

public class ComS2S
{
    [Flags]
    public enum EStreamOptions
    {
        None = 0x0,
        Encrypted = 0x1,
        Compressed = 0x2,
    }

    public class Options
    {
        public Options()
        {
            JobSystemOptions = new JobSystemOptions.Builder().Build();
        }
        
        public JobSystemOptions JobSystemOptions { get; set; }

        public EStreamOptions StreamOptions = EStreamOptions.None;
    }

    public delegate void OnDataReceivedEvent(byte[] data);
    public event OnDataReceivedEvent OnDataReceived;

    private ServiceCollection Services = new ServiceCollection();

    public ComS2S(Options opts = null)  
    {
        Services.AddService<Options>((IServiceProvider services) => {
            return opts != null ? opts : new Options();
        });
        Services.AddService<JobSystemOptions>((IServiceProvider services) => {
            return services.GetService<Options>().JobSystemOptions;
        });
        Services.AddService<JobSystem.JobSystem>((IServiceProvider services) => {
            return new JobSystem.JobSystem(services.GetService<JobSystem.JobSystemOptions>());
        });
    }

    public void SendData(byte[] data)  
    {
        IDataStream dataStream = new BufferDataStream();

        Options opts = Services.GetService<Options>();

        if(opts.StreamOptions.HasFlag(EStreamOptions.Compressed))
        {
            dataStream = new Compression(dataStream);
        }
        if(opts.StreamOptions.HasFlag(EStreamOptions.Encrypted))
        {
            dataStream = new Encryption(dataStream);
        }

        dataStream.WriteData(data);
        Console.WriteLine("Local: Data sent: " + Encoding.UTF8.GetString(data));
        Services.GetService<JobSystem.JobSystem>().AddJob(new SendDataJob(dataStream))
            .Then(TriggerOnDataReceived);
    }

    private void TriggerOnDataReceived(IDataStream stream)  
    {
        OnDataReceived?.Invoke(stream.ReadData());
    }

    public void Close()  
    {
        Services.GetService<JobSystem.JobSystem>().Stop();
    }
}

internal class SendDataJob: IJob<IDataStream>
{
    private IDataStream DataToSend;

    public SendDataJob(IDataStream dataToSend)
    {
        DataToSend = dataToSend;
    }

    public IDataStream Execute()
    {
        Console.WriteLine("Remote: Data received: " + Encoding.UTF8.GetString(DataToSend.ReadData()));
        Thread.Sleep(new Random().Next(300, 800));
        byte[] resp = Encoding.UTF8.GetBytes("a");
        Console.WriteLine("Remote: Data sent: " + Encoding.UTF8.GetString(resp));
        IDataStream stream = new BufferDataStream();
        stream.WriteData(resp);
        return stream;
    }
}
