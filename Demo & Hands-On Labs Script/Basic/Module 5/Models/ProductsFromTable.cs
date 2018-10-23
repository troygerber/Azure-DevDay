using Microsoft.WindowsAzure.Storage.Table; 
using System.ComponentModel.DataAnnotations;

public class ProductsFromTable : TableEntity 


#Constructors
public ProductsFromTable(string name, string category) 
{ 
    PartitionKey = category; RowKey = name; 
}
public ProductsFromTable() { }

#Properties

[Key] public string id { get; set; }  
public string ProductModel { get; set; } 
public string Description { get; set; } 