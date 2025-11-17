using Autofac;
using AutomationTask.Configuration;
using AutomationTask.Core.UI;
using AutomationTask.Core.Reporting;
using AutomationTask.Pages;
using Microsoft.Playwright;
using System.Reflection;

namespace AutomationTask.Core.DI;

public class TestContainerBuilder
{
    public static IContainer BuildContainer()
    {
        var builder = new ContainerBuilder();

        // Register configuration
        var settings = ConfigurationManager.GetTestSettings();
        builder.RegisterInstance(settings).SingleInstance();

        // Register UI components
        builder.RegisterType<BrowserFactory>().As<IBrowserFactory>().InstancePerLifetimeScope();
        builder.RegisterType<PageContext>().As<IPageContext>().InstancePerLifetimeScope();

        // Register IPage as a delegate that resolves from IPageContext
        // This allows page objects to receive IPage via DI after PageContext is initialized
        builder.Register(c => c.Resolve<IPageContext>().Page).As<IPage>().InstancePerLifetimeScope();

        // Auto-register all Page classes that inherit from BasePage
        // Autofac will inject IPage and TestSettings automatically
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BasePage)))
            .AsSelf()
            .InstancePerLifetimeScope();

        // Register reporting
        builder.RegisterType<ReportManager>().As<IReportManager>().SingleInstance();

        return builder.Build();
    }
}
