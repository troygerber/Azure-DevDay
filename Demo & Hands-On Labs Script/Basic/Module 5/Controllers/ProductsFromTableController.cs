using Microsoft.Azure; 
using Microsoft.WindowsAzure.Storage; 
using Microsoft.WindowsAzure.Storage.Table;

CloudStorageAccount storageAccount;
CloudTableClient tableClient;
CloudTable table;

public ProductsFromTableController()
{ 
    storageAccount = CloudStorageAccount.Parse( CloudConfigurationManager.GetSetting("[connectString]"));
    tableClient = storageAccount.CreateCloudTableClient();
    table = tableClient.GetTableReference( CloudConfigurationManager.GetSetting( "tableName"));
}

public ActionResult Index() 
{ 
    TableQuery<ProductsFromTable> query = new TableQuery<ProductsFromTable>();
    List<ProductsFromTable> products = new List<ProductsFromTable>(); 
    TableContinuationToken token = null; 
    do 
    { 
        TableQuerySegment<ProductsFromTable> resultSegment = table.ExecuteQuerySegmented(query, token); 
        token = resultSegment.ContinuationToken; 
        foreach (ProductsFromTable product in resultSegment.Results) 
        {    
            products.Add(product); 
        } 
    } 
    while (token != null); 
    return View(products); 
}

public ActionResult Create() 
{ 
    return View(); 
}

[HttpPost] [ValidateAntiForgeryToken] public ActionResult Create([Bind(Include = "PartitionKey, RowKey, id, ProductModel, Description")] ProductsFromTable product) 
{ 
    if (ModelState.IsValid) 
    { 
        TableOperation insertOperation = TableOperation.Insert(product);
        table.Execute(insertOperation); 
    } 
    return RedirectToAction("Index"); 
}