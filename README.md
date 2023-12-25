# AzureFundamentals

![image](https://github.com/mfkimbell/azure-fundamentals/assets/107063397/cefdc5d9-ac36-429e-83eb-81fe93c7a938)

``` C#
await foreach (BlobItem blobItem in _blobContainer.GetBlobsAsync())
{
    // get metadata (_blobClient is the blob service client)
    var blobClient = _blobContainer.GetBlobClient(blobItem.Name);
    BlobProperties blobProperties = await blobClient.GetPropertiesAsync();
    string blobToAdd = blobItem.Name;
    if (blobProperties.Metadata.ContainsKey("title"))
    {
        blobToAdd += "(" + blobProperties.Metadata["title"] + ")";
    }
    containerNamesAndBlobNames.Add("------"+blobToAdd);
```

![image](https://github.com/mfkimbell/azure-fundamentals/assets/107063397/0581b1ab-dbe8-4e8a-825d-59f53e9b6702)

generate shared access token

![image](https://github.com/mfkimbell/azure-fundamentals/assets/107063397/2d3559ac-bf80-4491-9d44-cedf7c9fffd7)

same action through code, we add the access token string to the end of the URI

``` c#
string sasContainerSignature = "";

if (blobContainerClient.CanGenerateSasUri)
{
    BlobSasBuilder sasBuilder = new()
    {
        BlobContainerName = blobContainerClient.Name,
        Resource = "c", // c for container
        ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
    };

    sasBuilder.SetPermissions(BlobSasPermissions.Read);

    sasContainerSignature = blobContainerClient.GenerateSasUri(sasBuilder).AbsoluteUri.Split('?')[1].ToString();
}

await foreach (var item in blobs)
{
    var blobClient = blobContainerClient.GetBlobClient(item.Name);
    Blob blobIndividual = new()
    {
        Uri = blobClient.Uri.AbsoluteUri + "?" + sasContainerSignature
    };
```


