using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

using Microsoft.AspNetCore.Http;

namespace WebApi.Controllers
{
    [Route("customers")]
    public class CustomerController : Controller
    {
        static Dictionary<long, Customer> customers = new Dictionary<long, Customer>();
        static Random rand = new Random();
        [HttpGet("{id:long}")]   
        public ActionResult GetCustomerAsync([FromRoute] long id)
        {
            if (customers.TryGetValue(id, out Customer customer))
                return new OkObjectResult(customer);
            else
                return new NotFoundResult();
        }

        [HttpPost("CreateCustomer")]   
        public ActionResult CreateCustomerAsync([FromBody] Customer customer)
        {
            if (customers.TryAdd(customer.Id, customer))
                return new OkObjectResult(customer.Id);
            else
                return new ConflictResult();
        }
        [HttpGet("All")]
        public ActionResult GetllCustomersAsync()
        {
            return new OkObjectResult(customers.Values.ToList());
        }

    }
}