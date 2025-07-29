using Business;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using CustomerEntity = DataAccess.Data.Customer;

namespace API.Controllers.Customer
{
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private BaseService<CustomerEntity> CustomerService;
        public CustomerController(BaseService<CustomerEntity> customerService)
        {
            CustomerService = customerService;
        }


        [HttpGet()]
        public IQueryable<CustomerEntity> GetAll()
        {
            return CustomerService.GetAll();
        }


        [HttpPost()]
        public CustomerEntity Create([FromBodyAttribute] CustomerEntity entity)
        {
            return CreateCustomer(entity);
        }

        private CustomerEntity CreateCustomer(CustomerEntity entity)
        {
           // throw new Exception("");
            return CustomerService.Create(entity);
        }

        [HttpPut()]
        public CustomerEntity Update(CustomerEntity entity)
        {
            return CustomerService.Update(entity.CustomerId, entity, out bool changed);
        }

        [HttpDelete()]
        public CustomerEntity Delete([FromBodyAttribute] CustomerEntity entity)
        {
            return CustomerService.Delete(entity);
        }
    }
}
