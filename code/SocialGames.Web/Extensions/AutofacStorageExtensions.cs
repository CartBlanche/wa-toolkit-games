using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Microsoft.Samples.SocialGames.Common.Storage;
using Microsoft.WindowsAzure;
using Autofac.Builder;
using Autofac.Features.LightweightAdapters;

namespace Microsoft.Samples.SocialGames.Extensions
{
    public static class AutofacStorageExtensions
    {

        public static IRegistrationBuilder<AzureQueue<TMessage>, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterQueue<TMessage>(this ContainerBuilder builder, string name) where TMessage : AzureQueueMessage
        {
            return builder.RegisterType<AzureQueue<TMessage>>().WithParameter("queueName", name);
        }

        public static IRegistrationBuilder<AzureBlobContainer<TEntity>, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterBlob<TEntity>(this ContainerBuilder builder, string name)
        {
            return builder.RegisterType<AzureBlobContainer<TEntity>>().WithParameter("containerName", name);
        }

        public static IRegistrationBuilder<AzureBlobContainer<TEntity>, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterBlob<TEntity>(this ContainerBuilder builder, string name, bool jsonpSupport)
        {
            return builder.RegisterType<AzureBlobContainer<TEntity>>().WithParameter("containerName", name).WithParameter("jsonpSupport", jsonpSupport);
        }

    }
}