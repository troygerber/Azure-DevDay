using System.Threading.Tasks; 

[ActionName("Index")] public async Task<ActionResult> Index() 
{ 
    var items = await CosmosDbRepository<ProductsFromCosmos>.GetItemsAsync();        
    return View(items);
}

public ActionResult Create() 
{ 
    return View(); 
}

[HttpPost] [ActionName("Create")] [ValidateAntiForgeryToken] public async Task<ActionResult> CreateAsync([Bind(Include = "ProductID,Name,ProductModel,Culture,Description")] ProductsFromCosmos item) 
{ 
    if (ModelState.IsValid) 
    { 
        await CosmosDbRepository<ProductsFromCosmos>.CreateItemAsync(item); 
        return RedirectToAction("Index"); 
    } 
    return View(item); 
}