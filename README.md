# Microsoft.Azure.WebJobs.Extensions.SimpleInjector
SimpleInjector extensions for cleaner DI setup in Azure WebJobs

# Set up
    static void Main()
        {
            var config = new JobHostConfiguration();

          
                config.UseDevelopmentSettings();
            

            config.UseTimers();
            config.UseSimpleInjector(DITestRegistrations.Register, "I'm the additional param", new AsyncScopedLifestyle());

            var host = new JobHost(config);
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }  
          
          
          
The following code snippet shows the extension method on the `JobHostConfiguration` which takes in an `Action<object>` where your DI registrations should live. Passing it a `new AsyncScopedLifestyle` does not set any of the default lifestyles of the container. All this does is create an explicit scoped lifestyle when your functions are called and disposes your lifestyle. If your DI classes are all singletons then just pass it a `null` and the extensions will not bother creating explicit scopes. 

# Inject services into function
     // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([TimerTrigger("0 59 23 * * *", RunOnStartup = true)] //At 11:59PM
        TimerInfo info, ICancelWroBoxProcess process)
        {

                process.Begin();
        }
 In the above scenario, the `ICancelWroBoxProcess` will be injected. If you fail to register a service and attempt to inject it, you will receive an exception. Please create a unit test for your DI registrations. 
