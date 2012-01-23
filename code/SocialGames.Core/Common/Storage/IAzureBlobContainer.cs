namespace Microsoft.Samples.SocialGames.Common.Storage
{
    using System;
    using System.IO;

    public interface IAzureBlobContainer<T>
    {
        void EnsureExist();

        void EnsureExist(bool publicContainer);

        void Save(string objId, T obj);

        string SaveFile(string objId, byte[] content, string contentType);

        string SaveFile(string objId, byte[] content, string contentType, TimeSpan timeOut);

        T Get(string objId);

        Stream GetFile(string objId);

        void Delete(string objId);

        string GetSharedAccessSignature(string objId, DateTime expiryTime);

        void DeleteContainer();
    }
}