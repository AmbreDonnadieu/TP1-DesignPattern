using System;
using System.Threading;
using JobSystem;

public class ComS2S
{
    public class Options
    {
        public Options()
        {
            JobSystemOptions = new JobSystemOptions.Builder().Build();
        }
        
        public JobSystemOptions JobSystemOptions { get; set; }

        [Flags]
        public enum EStreamOptions
        {
            None = 0x0,
            Encrypted = 0x1,
            Compressed = 0x2,
        }

        EStreamOptions StreamOptions = EStreamOptions.None;
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
        Services.GetService<JobSystem.JobSystem>().AddJob(new SendDataJob(data))
            .Then(TriggerOnDataReceived);
    }

    private void TriggerOnDataReceived(byte[] data)  
    {
        OnDataReceived?.Invoke(data);
    }

    public void Close()  
    {
        Services.GetService<JobSystem.JobSystem>().Stop();
    }
}

internal class SendDataJob: IJob<byte[]>
{
    private byte[] DataToSend;

    public SendDataJob(byte[] dataToSend)
    {
        DataToSend = dataToSend;
    }

    public byte[] Execute()
    {
        Thread.Sleep(new Random().Next(300, 800));
        return new byte[10];
    }
}
