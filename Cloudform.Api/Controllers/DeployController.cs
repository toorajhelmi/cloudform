using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudform.Api.Models;
using Cloudform.Core.Arctifact;
using Microsoft.AspNetCore.Mvc;

namespace Cloudform.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeployController : ControllerBase
    {
        // GET api/values/5
        [HttpGet]
        public ActionResult<List<Build>> Get(int buildId, int lastEventId)
        {
            using (var context = new CloudformContext())
            {  
                var events = context.Builds.Where(build => build.BuildId == buildId && build.EventId > lastEventId);
                return events.ToList();
            }
        }

        // POST api/values
        [HttpPost]
        public ActionResult<int> Post([FromBody] Package package)
        {
           var factory = new Factory
            {
                Script = script,
                Props = new Dictionary<string, object> {
                    { "client_secret", "){BQ6{h>?-a568OG#))Y-n5V!|[b(^&" },
                    { "subscription_id", "9a4fe1a5-274e-4c67-8321-8a55ec1ea64d" },
                    { "client_id", "7ffb12bc-357e-46e5-83e2-7231372561a4" },
                    { "tenant_id", "bafa704d-560b-4ee8-9563-c265cae5ffe6" },
                    { "resource_group", "rg3245" },
                    { "region", "westus2" } },
            };

            var build = new Build();
            using (var context = new CloudformContext())
            {
                var buildEntity = context.Builds.Add(new Build());
                context.SaveChanges();
                factory.BuildId = buildEntity.Entity.BuildId;
            }

            Core.Builder.Build(factory, new EventLogger());
            return new OkObjectResult(factory.BuildId);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private const string script = @"
//the first param is type of the component and second it the name of the deployed component
//the lower case intake is the name of the component used to refer back to this component
//within this script
//input specifies what triggers the function which could be hhtp request, timer, or a queue
//output specifies what queues receive messages from the function.
//  -The request post body is placed in a variable called order
component(Function, ProcessOrder) processOrder size:F
    input(request, order), output(paymentQueue)
{
    //Runs a SQL SELECT statement on db 'Transaction' assigning the result is assigned to stock
    //Note that if result is a scalar (int, string, ...) you should use x = sql(Table) and if result 
    //is a list you should use sql(Table, x)
    stock = sql(Transaction)
    {
        select Available from Inventory
        where ItemId = [int]@order.ItemId
    }
    
    //Notice there is a @ behind a parameters. @ is used to indicate
    //the parameter is passed into the statement from the code at run time. 
    //You need to use @ for any non constant parameter on the right side of 
    //statements
    //
    //Note that we are running two SQL statements to retireive the same
    // row (above & below). This is for illustration. We could have used 
    // sql(Transaction, invenroty) to get all the values into inventory & then
    //use inventory.Quantity or inventory.OnHold to access these values.
    //
    onHold = sql(Transaction)
    {  
        select OnHold from Inventory
        where ItemId = [int]@order.ItemId
    }
    
    if (stock - onHold >= order.Quantity)
    {
        //Runs a SQL INSERT statement and assgined the new ID to order.Id
        order.Id = sql(Transaction)
        {
            insert into [Order] (CustomerId, ItemId, Quantity, [Status])
            output inserted.Id
            values ([uniqueidentifier]@order.CustomerId, [int]@order.ItemId, [int]@order.Quantity, 1)
        }
        
        //Runs a SQL INSERT statement on db 'Transaction'
        sql(Transaction)
        {
            insert into Hold (OrderId, Expiration)
            select [uniqueidentifier]@order.Id, DATEADD(minute, 5, GETDATE())
        }
        
        var newHold = onHold + order.Quantity;
        
        //Runs a SQL UPDATE statement
        sql(Transaction)
        {
            update Inventory
            set OnHold = [int]@newHold
            where Id = [int]@order.ItemId
        }
                 
        enqueue(paymentQueue, order)

        var processing = 
        {
            OrderId: order.Id,
            OrderStatus: 1
        };
        
        return processing
    }
    else
    {
        sql(Transaction)
        {
            update Order
            set Status = 5
            where Id = [uniqueidentifier]@order.Id
        }
        
        var processing = 
        {
            OrderId: order.Id,
            OrderStatus: 5
        };

        return processing
    }
}

component(SQL, Transaction) transactions
{
    Table([Customer])
    [
        Id          uniqueIdentifier    not null  default NewId()    PRIMARY KEY,
        FirstName   varchar(100),
        LastName    varchar(100),
        Email       varchar(100)        not null,
        Password    varchar(100)        not null
    ]
    
    Table([Order])
    [
        Id          uniqueIdentifier     not null  default NewId()    PRIMARY KEY,
        CustomerId  uniqueIdentifier     not null  REFERENCES [Customer] (Id),
        ItemId      int                  not null,
        Quantity    int                  not null,
        Status      int                  not null

    ]
    
    Table([OrderStatus])
    [
        Id          uniqueIdentifier     not null  default NewId()    PRIMARY KEY,
        StatusId    int                  not null,
        Name        varchar(50)          not null
    ]
    {
        insert into (StatusId, Name) values
            (1, 'Pending Payment'),
            (2, 'Pending Receipt'),
            (3, 'Payment Declined'),
            (4, 'Processed'),
            (5, 'Out Of Stock')
    }
    
    Table(Inventory)
    [
        Id          uniqueIdentifier     not null  default NewId()    PRIMARY KEY,
        ItemId      int                  not null,
        Available   int                  not null,
        OnHold      int                  not null
    ]
    
    Table(Hold)
    [
        Id          uniqueIdentifier     not null  default NewId()    PRIMARY KEY,
        OrderId     uniqueIdentifier     not null,
        Expiration  datetime             not null
    ]
}

component(Queue, payment) paymentQueue 

//Triggered when a new object is added to the payment queue.
component(Function, ProcessPayment) processPayment size:F
    input(queue, paymentQueue, order), output(receiptQueue)
{
    //code will place a placeholder function to be field by the developer.
    //-1st arg is the name of the function
    //-The rest are inputs to the function
    //-Returned value is assiged to a variable called processed
    processed = code(processPayment, order)
    
    if (processed)
    {
        sql(Transaction)
        {
            update Order
            set Status = 2
            where Id = [uniqueidentifier]@order.Id
        }
        
        //as ... is optional. Its instructs how the sql statement
        //should return the result. If absent, an array is returd.
        //Possible values for ''as'' are: 
        //-array: returns array of objects [Item, Item, ...]
        //-entity: returns a single object (e.g. item { Quanitity: 2})
        //-scalar: return a scalar value (e.g. 3, 'door')
        sql(Transaction, inventory) as entity
        {
            select Available, OnHold from Inventory
            where ItemId = [int]@order.ItemId
        } 
        
        var newAvailable = inventory.Available - order.Quantity;
        var newHold = inventory.OnHold - order.Quantity;
        
        sql(Transaction)
        {
            update Inventory
            set Available = [int]@newAvailable, OnHold = [int]@newHold
            where ItemId = [int]@order.ItemId
        }
        enqueue(receiptQueue, order)
    }
    else
    {
        sql(Transaction)
        {
            update Order
            set status = 3
            where Id = [uniqueidentifier]@order.Id
        }
    }
    
    sql(Transaction)
    {
        delete Hold
        where OrderId = [uniqueidentifier]@order.Id
    }
}

component(Function, CheckHolds) checkHolds 
    input(timer, 60) //Trigger every 60 seconds
{
    sql(Transaction, expiredHolds)
    {
        select * from Hold
        where Expiration < GETDATE()
    }
    
    //use iterate to loop on a list. 
    //if there is no await call inside the loop, iterate will act like
    //an normal forEach. Otherwise it will implement an async forEach.
    //
    //SQL statements are awaited so we need to use iterate. JS forEach will
    //not work
    expiredHolds.iterate((expiredHold) =>
    {
        sql(Transaction, order)
        {
            select * from [Order]
            where Id = [uniqueidentifier]@expiredHold.OrderId
        }
        
        sql(Transaction, inventory)
        {
            select * from Inventory
            where ItemId = [int]@order.ItemId
        }
        
        var newHold = inventory.OnHold - order.Quantity;
        
        sql(Transaction)
        {
            update Inventory 
            set OnHold = [int]newHold
            where ItemId = [int]@inventory.ItemId
        }
        
        sql(Transaction)
        {
            delete Hold
            where Id = [uniqueidentifier]@expiredHold.Id
        }  
    });
}

component(Queue, receipt) receiptQueue

component(Function, GenerateReceipt) generateReceipt size:F
    input(queue, receiptQueue ,order)
{
    //order = dequeue(receiptQueue)
    code(generateReceipt, order)
    sql(Transaction)
    {
        update Order
        set Status = 4
        where Id = [uniqueidentifier]@order.Id
    }
}








";

    }
}
