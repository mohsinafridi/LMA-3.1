﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMA.Data.Interfaces;
using LMA.Data.Model;
using LMA.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LMA.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IBookRepository _bookRepository;

        public CustomerController(ICustomerRepository customRepository,IBookRepository bookRepository)
        {
            _customerRepository = customRepository;
            _bookRepository = bookRepository;
        }
        [Route("Customer")]
        public IActionResult List()
        {
            var customerVM = new List<CustomerViewModel>();
            var customers = _customerRepository.GetAll();
            if (customers.Count() == 0)
            {
                return View("Empty");
            }
            foreach (var cust in customers)
            {
                customerVM.Add(new CustomerViewModel
                {
                    Customer = cust,
                    BookCount = _bookRepository.Count(x => x.CustomerId == cust.CustomerId)
                });
            }
            return View(customerVM);
        }

        public IActionResult Delete(int id)
        {
            var customer = _customerRepository.GetById(id);
            // Check whether this customer lend/borrow any book
            // if so,then don't delete him/her.
            var customerLendBook = _bookRepository.Count(x => x.CustomerId == id);
            if (customerLendBook > 0)
            {
                ViewData["Message"] = $"{customer.Name} has lend book,so can't delete.";
                //return Json($"{customer.Name} has lend book,so can't delete.");
                return RedirectToAction("List");
            }
            _customerRepository.Delete(customer);
            return RedirectToAction("List");
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            _customerRepository.Create(customer);
            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var customer  =_customerRepository.GetById(id);
            return View(customer);
        }
        [HttpPost]
        public IActionResult Update(Customer customer)
        {
            _customerRepository.Update(customer);
            return RedirectToAction("List");
        }
    }
}