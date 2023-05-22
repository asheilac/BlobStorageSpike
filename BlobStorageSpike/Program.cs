using Azure.Storage;
using Azure.Storage.Blobs;

const string blobContainerName = "blobbercontainer";

string connectionString = "";

var blobServiceClient = new BlobServiceClient(connectionString);

var blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);

// Create a local file in the ./data/ directory for uploading and downloading
string localPath = "data";
Directory.CreateDirectory(localPath);
string fileName = "quickstart" + Guid.NewGuid().ToString() + ".txt";
string localFilePath = Path.Combine(localPath, fileName);

// Write text to the file
await File.WriteAllTextAsync(localFilePath, "Hello, Mazu!");

// Get a reference to a blob
BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);

// Generate SAS Token 
Azure.Storage.Sas.BlobSasBuilder blobSasBuilder = new Azure.Storage.Sas.BlobSasBuilder()
{
    BlobContainerName = blobContainerName,
    BlobName = fileName,
    ExpiresOn = DateTime.UtcNow.AddMinutes(5)
};
blobSasBuilder.SetPermissions(Azure.Storage.Sas.BlobSasPermissions.Read);
var azureStorageAccount = "blobbercontainer";
var azureStorageAccessKey = "";
var sasToken = blobSasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(azureStorageAccount, azureStorageAccessKey)).ToString();

// Log link to uploaded content
var link = $"{blobClient.Uri}?{sasToken}";
Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", link);

// Upload data from the local file
await blobClient.UploadAsync(localFilePath, false);