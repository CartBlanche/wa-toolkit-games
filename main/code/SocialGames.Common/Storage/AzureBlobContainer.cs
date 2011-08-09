namespace Microsoft.Samples.SocialGames.Common.Storage
{
    using System;
    using System.IO;
    using System.Web.Script.Serialization;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    public class AzureBlobContainer<T> : IAzureBlobContainer<T>
    {
        private readonly CloudBlobContainer container;
        private readonly bool jsonpSupport;

        private static JavaScriptSerializer serializer;

        public AzureBlobContainer(CloudStorageAccount account)
            : this(account, typeof(T).Name.ToLowerInvariant(), false)
        {
        }

        public AzureBlobContainer(CloudStorageAccount account, bool jsonpSupport)
            : this(account, typeof(T).Name.ToLowerInvariant(), false)
        {
        }

        public AzureBlobContainer(CloudStorageAccount account, string containerName)
            : this(account, containerName.ToLowerInvariant(), false)
        { 
        }

        public AzureBlobContainer(CloudStorageAccount account, string containerName, bool jsonpSupport)
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }

            if (containerName == null)
            {
                throw new ArgumentNullException("containerName");
            }

            serializer = new JavaScriptSerializer();

            var client = account.CreateCloudBlobClient();
            client.RetryPolicy = RetryPolicies.Retry(3, TimeSpan.FromSeconds(5));

            this.jsonpSupport = jsonpSupport;

            this.container = client.GetContainerReference(containerName);
        }

        public void EnsureExist()
        {
            this.container.CreateIfNotExist();
        }

        public void EnsureExist(bool publicContainer)
        {
            this.container.CreateIfNotExist();
            var permissions = new BlobContainerPermissions();

            if (publicContainer)
            {
                permissions.PublicAccess = BlobContainerPublicAccessType.Container;
            }
            
            this.container.SetPermissions(permissions);
        }

        public void Save(string objId, T obj)
        {
            CloudBlob blob = this.container.GetBlobReference(objId);
            blob.Properties.ContentType = "application/json";

            var serialized = string.Empty;
            serialized = serializer.Serialize(obj);

            if (this.jsonpSupport)
            {
                serialized = this.container.Name + "Callback(" + serialized + ")";
            }

            blob.UploadText(serialized);
        }

        public string SaveFile(string objId, byte[] content, string contentType)
        {
            CloudBlob blob = this.container.GetBlobReference(objId);
            blob.Properties.ContentType = contentType;
            blob.UploadByteArray(content);
            return blob.Uri.ToString();
        }

        public string SaveFile(string objId, byte[] content, string contentType, TimeSpan timeOut)
        {
            TimeSpan currentTimeOut = this.container.ServiceClient.Timeout;
            this.container.ServiceClient.Timeout = timeOut;
            string result = this.SaveFile(objId, content, contentType);
            this.container.ServiceClient.Timeout = currentTimeOut;
            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "If we dispose the stream the clien won't be able to use it")]
        public Stream GetFile(string objId)
        {
            Stream stream = new MemoryStream();
            CloudBlob blob = this.container.GetBlobReference(objId);
            blob.DownloadToStream(stream);
            stream.Seek(0, 0);
            return stream;
        }

        public T Get(string objId)
        {
            CloudBlob blob = this.container.GetBlobReference(objId);

            try
            {
                var serialized = blob.DownloadText();

                if (this.jsonpSupport)
                {
                    serialized = serialized.Replace(this.container.Name + "Callback(", string.Empty);
                    serialized = serialized.Remove(serialized.Length - 1, 1);
                }

                return serializer.Deserialize<T>(serialized);
            }
            catch (StorageClientException)
            {
                return default(T);
            }
        }

        public void Delete(string objId)
        {
            CloudBlob blob = this.container.GetBlobReference(objId);
            blob.DeleteIfExists();
        }

        public void DeleteContainer()
        {
            this.container.Delete();
        }

        public string GetSharedAccessSignature(string objId, DateTime expiryTime)
        {
            CloudBlob blob = this.container.GetBlobReference(objId);

            var blobAccess = new SharedAccessPolicy
            {
                Permissions = SharedAccessPermissions.Read,
                SharedAccessExpiryTime = expiryTime
            };

            return blob.Uri + blob.GetSharedAccessSignature(blobAccess);
        }
    }
}