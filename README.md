# Azure Container and Blob Management



### **Tools Used:**
*  `Azure blob storage`
*  `c#`
*  `.NET 8`
* `blobServiceClient`


### Purpose:
The purpose of this application is to edit, add, and delete containers and blobs on Azure from a .Net application, as well as display images from a specified container. 



#### Add Blob
<img width="680" alt="Screenshot 2023-11-26 at 12 07 45 PM" src="https://github.com/mfkimbell/azure-container-and-blob-management/assets/107063397/bf532908-0d7f-40ab-8388-300372212709">

#### Delete Blob
![deleteBlob](https://github.com/mfkimbell/azure-container-and-blob-management/assets/107063397/dba09275-0b31-41e7-8b00-2200458a54f5)

#### Create Container
<img width="680" alt="Screenshot 2023-11-26 at 12 07 45 PM" src="https://github.com/mfkimbell/azure-container-and-blob-management/assets/107063397/0d431bb3-4cf0-4b19-943e-11f679dec7e8">

#### Delete Container
<img width="680" alt="Screenshot 2023-11-26 at 12 07 45 PM" src="https://github.com/mfkimbell/azure-container-and-blob-management/assets/107063397/01b35695-f7f4-40fe-a374-c0d926af33ac">

#### Metadata Display
<img width="680" alt="Screenshot 2023-11-26 at 12 07 45 PM" src="https://github.com/mfkimbell/azure-container-and-blob-management/assets/107063397/7fd1b5fd-eb7a-4200-b75c-8487a87d216b">





We use blobServiceClient C# package to manage, delete, and add containers. 

In the main page, we display the current heiarchy of containers and blobs: 

``` C#
<div class="p-4">
    <h4 class="text-primary"> Blob Storage Heiarchy</h4>

    @foreach(var item in Model)
    {
        <p>@item</p>
    }
</div>
```

Our “homeController” is responsible for rendering all current Containers and Blobs on startup. We have a for loop in Index.cshtml that renders all of the items. To create that list, in the ContainerService, we have “GetAllContainersAndBlobs” which adds containers then a nested loop that gets the blobs for each container

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

<img width="609" alt="Screenshot 2023-11-26 at 12 07 45 PM" src="https://github.com/mfkimbell/azure-container-and-blob-management/assets/107063397/00c5abeb-564f-495f-a1dc-440066fc8c76">

When "container" is selected, 


<img width="609" alt="Screenshot 2023-11-26 at 12 07 45 PM" src="https://github.com/mfkimbell/azure-container-and-blob-management/assets/107063397/08fcae88-fafb-436a-87af-fd7ca89315eb">


In index.cshtml, we have buttons that do “asp-actions” and they lead to functions in the "asp-controller" designated controller. 

The controller will return a “view” with the data passed in, and our frontend will update.

We create interfaces for blobService and containerService to outline basic commands like “get all blobs” “get one blob” “delete blob” etc…
We make “blobService.cs” and “containerService.cs” with functions that will make our controller’s code more clean and simple.

We implement views for each of the controller’s functions to update the screen with new info. Using these functions, CRUD functions, I can alter data on Azure account with code. The functions execute, then the view is refrehsed with the updated data.


Files receive a hash when inputted to prevent overwriting of duplicate names:

``` C#
 var fileName = Path.GetFileNameWithoutExtension(file.FileName)+"_"+Guid.NewGuid()+Path.GetExtension(file.FileName);
```


We display the images in the images tab with the public URI, this doesn’t work on privagte containers (obviously), but we can use a shared access signature (SAS) to get a working URI.

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

To to add comment and title, we specify meta data on the blob object upon creation:

``` c#
 BlobProperties blobProperties = await blobClient.GetPropertiesAsync();
                if (blobProperties.Metadata.ContainsKey("title"))
                {
                    blobIndividual.Title = blobProperties.Metadata["title"];
                }
                if (blobProperties.Metadata.ContainsKey("comment"))
                {
                    blobIndividual.Comment = blobProperties.Metadata["comment"];
                }
                blobList.Add(blobIndividual);
            }
            return blobList;
```
And we reuse that metadata when displaying titiles in the file heiarchy and in the images page:

``` c#
<div class="row">
        @foreach(var item in Model)
        {
            <div class="col-4 border p-2">
                <h4 class="text-success">@item.Title</h4>
                <img src="@item.Uri" width="100%" />
			</div>
        }
    </div>
```

