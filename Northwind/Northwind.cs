using System;
using System.Collections.Generic;

namespace dotObjects.Northwind
{
    #region Products
    partial class Category
    {
        public Category(string name, string description, List<Product> products)
            : this()
        {
            CategoryName = name;
            Description = description;
            Products.AddRange(products);
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", CategoryName, Description);
        }
    }

    partial class Product
    {
        public Product(string name, Supplier supplier, string quantityPerUnit, decimal unitPrice, short unitsInStock)
            : this()
        {
            ProductName = name;
            Supplier = supplier;
            QuantityPerUnit = quantityPerUnit;
            UnitPrice = unitPrice;
            UnitsInStock = unitsInStock;
        }

        public void ChangeCategory(Category category)
        {
            if (category == null)
                throw new ArgumentException("The category cannot be empty.");
            Category = category;
        }

        public override string ToString()
        {
            return string.Format("{0:000} - {1}", ProductID, ProductName);
        }
    }
    #endregion

    #region Ordering
    partial class Order
    {
        public Order(DateTime orderDate, DateTime requiredDate, List<OrderItem> items) : this()
        {
            OrderDate = orderDate;
            RequiredDate = requiredDate;
            Items.AddRange(items);
        }

        public override string ToString()
        {
            return string.Format("{0} - {1:dd/MM/yyyy} ({2})", OrderID, OrderDate, Customer);
        }
    }

    partial class OrderItem
    {
        public OrderItem(Product product, short quantity, decimal unitPrice, float discount)
            : this()
        {
            Product = product;
            Quantity = quantity;
            UnitPrice = unitPrice;
            Discount = discount;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1} x {2:0.00})", Product, Quantity, UnitPrice);
        }
    }
    #endregion

    #region Entries (Customer, Shipper, Supplier, Employee)
    partial class Customer
    {
        public override string ToString()
        {
            return string.Format("{0:000} - {1}", CustomerID, CompanyName);
        }
    }

    partial class Shipper
    {
    }

    partial class Supplier
    {

        public override string ToString()
        {
            return CompanyName;
        }
    }

    partial class Employee
    {
        public override string ToString()
        {
            return string.Format("{0} {1} - {2}", FirstName, LastName, Extension);
        }
    }
    #endregion

    #region Regions and Territories
    partial class Region
    {
        public Region(string description)
            : this()
        {
            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("Description cannot be empty.");
            RegionDescription = description;
        }

        public override string ToString()
        {
            return string.Format("{0}", RegionDescription);
        }
    }

    partial class Territory
    {
        public Territory(string id, string description)
            : this()
        {
            TerritoryID = id;
            TerritoryDescription = description;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", TerritoryID, TerritoryDescription);
        }
    }
    #endregion
}
