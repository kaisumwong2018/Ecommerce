// See https://aka.ms/new-console-template for more information
using AdvanceATM.Model;
using ConsoleTables;
using Ecommerce.Model;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;


AppDbContext db = new AppDbContext();

Address newaddress = new Address();
newaddress.Address1 = "Address1";
db.Address.Add(newaddress);
db.SaveChanges();

while (true)
{
    Console.WriteLine("2.Login");
    string opt = Console.ReadLine()!;
    switch (opt)
    {
        case "1":

            #region Admin

            Console.WriteLine("Create New User");
            Customer newcustomer = new Customer();

            Console.WriteLine("Enter User Name");
            newcustomer.UserName = Console.ReadLine();

            Console.WriteLine("Enter Password");
            newcustomer.Password = Console.ReadLine();
            newcustomer.LoginTry = 3;
            newcustomer.LastLoginTime = DateTime.Now;
            newcustomer.Status = "Active";

            newcustomer.AddressId = db.Address.LastOrDefault()!.AddressId;
            db.customers.Add(newcustomer);
            db.SaveChanges();

            #endregion

            break;

            case "2":

            #region Customer Login

            Console.WriteLine("Login");
            var finduser = findcustomer();

            Console.WriteLine("Welcome Back " + finduser.UserName + " Last Login Time " + finduser.LastLoginTime);
            finduser.LastLoginTime = DateTime.Now;
            finduser.LoginTry = 3;
            db.SaveChanges();

            Console.WriteLine("1.Display All Active Products");
            Console.WriteLine("2.Please Enter Your Option");
            string customeropt = Console.ReadLine()!;

            switch (customeropt)
            {
                case "1":

                    producttable();

                    break;

                    case "2":

                    #region Add to cart?

                    while (true)
                    {

                        producttable();
                        Console.WriteLine("Please Enter ProductId to Add to Cart");
                        string productid = Console.ReadLine()!;

                        var searchproduct = db.products.FirstorDefault(x => x.ProductId.ToString == productid);

                        if (searchproduct == null)
                        {
                            Console.WriteLine("Please Enter Valid Product Id");
                            continue;
                        }

                        Console.WriteLine("Please Enter Quantity");
                        int qty = Convert.ToInt32(Console.ReadLine())!;
                        var checkcart = db.carts.FirstOrDefault(x => x.ProductId == searchproduct.ProductsId
                        && x.CustomerId == finduser.CustomerId
                        && x.CartStatus == "Pending");

                        if(checkcart == null)
                        {
                            checkcart.Quantity += qty;
                        }

                        Cart newcart = new Cart();
                        newcart.ProductId = searchproduct.ProductId;
                        newcart.Quantity = qty; 
                        newcart.CustomerId = finduser.CustomerId;
                        newcart.CartStatus = "Pending";
                        db.carts.Add(newcart);
                        db.SaveChanges();
                        break;
                    }
                    #endregion

                    break;

                    case "3":

                    #region Show Cart to Checout / remove

                    var findcart = (from c in db.carts
                                    join p in db.products
                                    on c.ProductId equals p.ProductId
                                    where c.CustomerId == finduser.CustomerId && c.CartStatus == "Pending"
                                    select new { c, p }).Tolist();
                    var carttable = new ConsoleTable("ID", "Prodcut Name", "Quantity", "Price", "Subtotal");
                    foreach (Cart p in findcart)
                    {
                        carttable.AddRow(p.Id,p.Product.ProductName,p.Quantity,p.Product.DiscountPrice,p.Quantity*p.Product.Discount);
                    }
                    carttable.AddRow("","","","Total",findcart.Sum(x=>x.Quantity*x.Product.DiscountPrice));
                    carttable.Write();

                    Console.WriteLine("[1]Checkout | [2] Remove Selected Products | [3] Remove All Products");
                    string cartopt = Console.ReadLine()!;
                    switch (cartopt)
                    {
                        case "1":

                            Console.WriteLine("Please Enter Card Number");
                            string cardno = Console.ReadLine()!;

                            Console.WriteLine("Please Enter PIN Number");
                            string pinno = Console.ReadLine()!;

                            if(cardno = "4242424242424242" %% pinno = "123456")
                            {
                                Console.WriteLine("Payment Successful");
                                for(int i; i< findcart.Count(); i++)
                                {
                                    OrderDetail neworderdetail = new OrderDetail();
                                    neworderdetail.ProductName = findcart[i].p.ProductName;
                                    neworderdetail.DiscountPrice = findcart[i].p.DiscountPrice;
                                    neworderdetail.CustomerId = finduser.CustomerId;
                                    neworderdetail.PaymentDate = DateTime.Now;
                                    db.orderDetails.Add(neworderdetail);
                                    db.SaveChanges();
                                    orderlist = orderlist + "," + db.orderDetails.OrderBy(x=>x.OrderDetailId).Last().OrderDetailId;
                                }
                            
                            }

                            //This is the function for remove cart
                            var removecart = db.cart.Where(c => c.CustomerId == finduser.CustomerId && c.CartStatus == "Pending");
                            db.RemoveRange(removecart); 

                            TransactionHistory newth = new TransactionHistory();
                            newth.OrderList = orderlist;
                            newth.CustomerId = finduser.CustomerId;
                            newth.Subtotal = findcart.Sum(x => x.c.Quantity * x.p.DiscountPrice);
                            newth.Tax = newth.Subtotal * Convert.ToDecimal(0.06);
                            newth.grandtotal = newth.Subtotal + newth.Tax;
                            db.transactionHistories.Add(newth);
                            db.SaveChanges();

                            else
                            {
                                Console.WriteLine("Payment Fail");
                            }

                            break;

                            case "2":
                            break;

                            case "3":
                            break;
                    }




                    #endregion

                    break;
            }

            #endregion

            break;

    }

   
}

void producttable()
{
    var producttable = new ConsoleTable("ID", "Product Name", "Price", "Discount Price", "Stock");

    foreach (var p in db.products)
    {
        producttable.AddRow(p.ProductsId, p.ProductName, p.ProductPrice, p.DiscountPrice, p.Stock);
    }

    producttable.Write();
    Console.ReadKey();
}

Customer findcustomer()
{
    while (true)
    {
     
        Console.WriteLine("Enter User Name:");
        string username = Console.ReadLine()!;

        Console.WriteLine("Enter Password");
        string password = Console.ReadLine()!;

        var findusername = db.customers.FirstOrDefault(x => x.UserName == username);

        if (findusername == null)
        {
            Console.WriteLine("No User Found");
            continue;
        }

        var finduser = db.customers.FirstOrDefault(x => x.UserName == username
        && x.Password == password);

        if (finduser == null)
        {
            Console.WriteLine("Wrong Password");
            findusername.LoginTry--;
            if (findusername.LoginTry <= 0)
            {
                findusername.LoginTry = 0;
                findusername.Status = "Suspend";
                Console.WriteLine("Account Suspended");
            }
            db.SaveChanges();
            continue;
        }
        return finduser;
    }
    
}




















