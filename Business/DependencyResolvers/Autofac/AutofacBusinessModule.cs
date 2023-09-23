using Autofac;
using Business.Abstract;
using Business.Abstract.General;
using Business.Concrete;
using Business.Concrete.General;

namespace Business.DependencyResolvers.Autofac
{
    public class AutofacBusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DirectoryManager>().As<IDirectoryService>();
            builder.RegisterType<SoundManager>().As<ISoundService>();

            builder.RegisterType<SoundboardComManager>().As<ISoundboardComService>();
        }
    }
}
