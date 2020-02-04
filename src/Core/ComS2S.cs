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
    }

    ServiceCollection Services = new ServiceCollection();

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

    public void Close()  
    {
        Services.GetService<JobSystem.JobSystem>().Stop();
    }
}
