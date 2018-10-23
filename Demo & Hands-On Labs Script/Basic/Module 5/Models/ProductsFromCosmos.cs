using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

[Key] [JsonProperty(PropertyName = "Product Id")] 
public string ProductId { get; set; } [JsonProperty(PropertyName = "Name")] 
public string Name { get; set; } [JsonProperty(PropertyName = "Product model")] 
public string ProductModel { get; set; } [JsonProperty(PropertyName = "Culture")] 
public string Culture { get; set; } [JsonProperty(PropertyName = "Description")] 
public string Description { get; set; }