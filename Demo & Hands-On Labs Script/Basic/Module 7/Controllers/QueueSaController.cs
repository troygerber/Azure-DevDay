using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

private CloudQueue queue;

public QueueSaController() 
{ 
    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("[connectionString]")); 
    CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient(); 
    queue = queueClient.GetQueueReference(CloudConfigurationManager.GetSetting("queueName")); 
    queue.CreateIfNotExists();
}

private int? GetTotalOfMessage() 
{ 
    queue.FetchAttributes(); return queue.ApproximateMessageCount;
}

public ActionResult Index() 
{ 
    int total = GetTotalOfMessage() ?? 0; 
    return View(new Models.QueueSa() 
    { 
        TotalOfMessages = total, Messages = queue.GetMessages(total == 0 ? 1 : total, new TimeSpan(5000)) 
    }); 
}

 [HttpPost] public ActionResult AddMessage(string message) 
 { 
     queue.AddMessage(new CloudQueueMessage(string.Format(message))); 
     return View("Index", new Models.QueueSa()
    { 
         TotalOfMessages = GetTotalOfMessage() ?? 0, 
        Messages = queue.GetMessages(GetTotalOfMessage() ?? 0, new TimeSpan(1000)) 
    }); 
}

public ActionResult PeekMessage() 
{ 
    int total = GetTotalOfMessage() ?? 0; 
    CloudQueueMessage peekMessage = null;
    peekMessage = queue.PeekMessage();
    if (peekMessage != null) 
    { 
        return View("Index", new Models.QueueSa() 
        { 
            TotalOfMessages = total, 
            Message = string.Format("PEEK: {0}", peekMessage.AsString),
            Messages = queue.GetMessages(total == 0 ? 1 : total, new TimeSpan(1000)) 
        }); 
    } 
    return View("Index", new Models.QueueSa() 
    {   
        TotalOfMessages = total, Message = "PEEK: There are no messages." 
    }); 
}

public ActionResult GetMessage() 
{ 
    int total = 0; 
    CloudQueueMessage getMessage = null; 
    getMessage = queue.GetMessage(); 
    if (getMessage != null) 
    { 
        queue.DeleteMessage(getMessage); 
        total = GetTotalOfMessage() ?? 0; 
        return View("Index", new Models.QueueSa() 
        { 
            TotalOfMessages = total, 
            Message = string.Format("GET: {0}", getMessage.AsString),
            Messages = queue.GetMessages(total == 0 ? 1 : total, 
            new TimeSpan(1000)) 
        }); 
    } 
    return View("Index", new Models.QueueSa() 
    { 
        TotalOfMessages = total, Message = "GET: There are no messages." 
    });
}

[HttpPost] public ActionResult UpdateMessage([Bind(Include = "message")]string message) 
{ 
    int total = 0; CloudQueueMessage getMessage = null; 
    getMessage = queue.GetMessage(); 
    if (getMessage != null) 
    { 
        getMessage.SetMessageContent(message); 
        queue.UpdateMessage(getMessage, TimeSpan.FromSeconds(1), 
        MessageUpdateFields.Content | MessageUpdateFields.Visibility); 
        
        total = GetTotalOfMessage() ?? 0; 
        return View("Index", new Models.QueueSa() 
        { 
            TotalOfMessages = total, Message = string.Format("UPDATE: {0}", getMessage.AsString), 
            Messages = queue.GetMessages(total == 0 ? 1 : total, new TimeSpan(1000)) 
        }); 
    } 
    return View("Index", new Models.QueueSa() 
    { 
        TotalOfMessages = total, Message = "UPDATE: There are no messages."
    }); 
}