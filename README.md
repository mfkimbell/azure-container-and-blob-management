# Azure Container and Blob Management



### **Tools Used:**
*  `Azure blob storage`
*  `c#`
*  `.NET 8`
* `blobServiceClient`


### Purpose:
The purpose of this application is to edit, add, and delete containers and blobs on Azure from a .Net application, as well as display images from a specified container. 

### Overview:

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

![image](https://github.com/mfkimbell/azure-container-and-blob-management/assets/107063397/08fcae88-fafb-436a-87af-fd7ca89315eb)


In index.cshtml, we have buttons that do “asp-actions” and they lead to functions in the "asp-controller" designated controller. 

The controller will return a “view” with the data passed in, and our frontend will update.

We create interfaces for blobService and containerService to outline basic commands like “get all blobs” “get one blob” “delete blob” etc…
We make “blobService.cs” and “containerService.cs” with functions that will make our controller’s code more clean and simple.

We implement views for each of the controller’s functions to update the screen with new info.




Files receive a hash when inputted to prevent overwriting of duplicate names

All actions in the html lead to routes in the controller where information is retrieved, updated, and a new view is pre-rendered. 


You can manually add meta data to blobs manually on Azure. 

We display the images in the images tab with the public URI, this doesn’t work on privagte containers (obviously), but we can use a shared access token to get the URI


NEW PROJECT FOR THIS, NEW HOME CONTROLLER

Azure function also in its own project

Our azure function sends a sales request to a queue

We make the trigger an HTTP, and copy the http executable

We use that http in the home controller, where we create a “post action method”




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


