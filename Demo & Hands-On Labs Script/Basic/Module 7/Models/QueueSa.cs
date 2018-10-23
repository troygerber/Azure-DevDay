using Microsoft.WindowsAzure.Storage.Queue; 

public int TotalOfMessages { get; set; } 
public string Message { get; set; } 
public IEnumerable<CloudQueueMessage> Messages { get; set; } 