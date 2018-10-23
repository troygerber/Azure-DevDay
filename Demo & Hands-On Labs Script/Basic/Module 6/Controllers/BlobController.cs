using Microsoft.Azure; 
using Microsoft.WindowsAzure.Storage; 
using Microsoft.WindowsAzure.Storage.Blob; 
using System.IO;

private CloudBlobContainer GetCloudBlobContainer() 
{ 
    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("[connectionString]"));
    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient(); 
    CloudBlobContainer container = blobClient.GetContainerReference( CloudConfigurationManager.GetSetting("containerName")); 
    container.CreateIfNotExists(); 
    return container;
} 

public ActionResult Index() 
{ 
    CloudBlobContainer container = GetCloudBlobContainer(); 
    List<string> blobs = new List<string>(); 
    foreach (IListBlobItem item in container.ListBlobs(useFlatBlobListing: true)) 
    { 
        if (item.GetType() == typeof(CloudBlockBlob)) 
        { 
            CloudBlockBlob blob = (CloudBlockBlob)item; blobs.Add(blob.Name); 
        } 
        else if (item.GetType() == typeof(CloudPageBlob)) 
        {
             CloudPageBlob blob = (CloudPageBlob)item; blobs.Add(blob.Name);
        } 
        else if (item.GetType() == typeof(CloudBlobDirectory)) 
        { 
            CloudBlobDirectory dir = (CloudBlobDirectory)item; blobs.Add(dir.Uri.ToString()); 
        } 
    } 
    return View(blobs); 
}


[HttpPost]
public ActionResult UploadBlob(HttpPostedFileBase file) 
{ 
    CloudBlobContainer container = GetCloudBlobContainer(); 
    CloudBlockBlob blob = container.GetBlockBlobReference(Path.GetFileName(file.FileName)); 
    using (var fileStream = file.InputStream) 
    { 
        blob.UploadFromStream(fileStream); 
    }
    GetContainerSasUri(blob); 
    return RedirectToAction("Index","Blob");
} 

public ActionResult DownloadBlob(string blobName) 
{ 
    CloudBlobContainer container = GetCloudBlobContainer(); 
    CloudBlockBlob blob = container.GetBlockBlobReference(blobName); 
    MemoryStream stream = new MemoryStream(); 
    blob.DownloadToStream(stream); 
    System.Web.HttpContext.Current.Response.ContentType = blob.Properties.ContentType.ToString(); 
    System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "Attachment; filename=" + blobName); 
    System.Web.HttpContext.Current.Response.AddHeader("Content-Length", blob.Properties.Length.ToString()); 
    System.Web.HttpContext.Current.Response.BinaryWrite(stream.ToArray()); 
    System.Web.HttpContext.Current.Response.Flush(); 
    System.Web.HttpContext.Current.Response.Close();
    return RedirectToAction("Index", "Blob");
}




private string GetContainerSasUri(CloudBlockBlob blob) 
{
     SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy(); 
     sasConstraints.SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddSeconds(10); 
     sasConstraints.Permissions = SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Write; 
     string sasContainerToken = blob.GetSharedAccessSignature(sasConstraints); 
     return blob.Uri + sasContainerToken; 
}