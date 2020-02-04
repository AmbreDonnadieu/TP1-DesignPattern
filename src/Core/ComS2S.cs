using JobSystem;

public class ComS2S
{
    ServiceCollection Services = new ServiceCollection();

    public ComS2S()  
    {
        Services.AddService<JobSystemOptions>((IServiceProvider services) => {
            return new JobSystemOptions.Builder().Build();
        });
        Services.AddService<JobSystem.JobSystem>((IServiceProvider services) => {
            return new JobSystem.JobSystem(services.GetService<JobSystem.JobSystemOptions>());
        });
    }

    public void Close()  
    {
        Services.GetService<JobSystem.JobSystem>().Stop();
    }
}
