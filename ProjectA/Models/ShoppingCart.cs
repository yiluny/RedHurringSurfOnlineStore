using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace ProjectA.Models
{
    public class ShoppingCart
    {
        ProjectDBContext storeDB = new ProjectDBContext();
        string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";
        

        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            
            cart.ShoppingCartId = cart.GetCartId(context);


            return cart;
        }

        // Helper method to simplify shopping cart calls
        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }


        public void AddToCart(ProductDB product, int num, string size)
        {
            // Get the matching cart and product instances
            var cartItem = storeDB.Carts.SingleOrDefault(
                c => c.CartId == ShoppingCartId
                && c.ProductId == product.productId);

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new Cart
                {
                    ProductId = product.productId,
                    CartId = ShoppingCartId,
                    Count = num,
                    ProductSize = size,
                    ProductSubTotal = num * product.productPrice,
                    DateCreated = DateTime.Now
                };


                storeDB.Carts.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart, 
                // then add one to the quantity
                cartItem.Count += num;
                cartItem.ProductSubTotal = cartItem.Count * (cartItem.Product.productPrice);
            }

            // Save changes
            storeDB.SaveChanges();
        }

        public int RemoveOne(int id)
        {
            // Get the cart
            var cartItem = storeDB.Carts.Single(
                cart => cart.CartId == ShoppingCartId
                && cart.RecordId == id);

            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    storeDB.Carts.Remove(cartItem);
                }
                // Save changes
                storeDB.SaveChanges();
            }
            return itemCount;
        }

        public int AddOne(int id)
        {
            // Get the cart
            var cartItem = storeDB.Carts.Single(
                cart => cart.CartId == ShoppingCartId
                && cart.RecordId == id);

            int itemCount = 0;

            if (cartItem != null)
            {
                cartItem.Count++;
                itemCount = cartItem.Count;

                // Save changes
                storeDB.SaveChanges();
            }
            return itemCount;

        }

        public int DeleteOne(int id)
        {
            var cartItem = storeDB.Carts.Single(
                cart => cart.CartId == ShoppingCartId
                && cart.RecordId == id);

            int itemCount = 0;

            storeDB.Carts.Remove(cartItem);

            // Save changes
            storeDB.SaveChanges();
            return itemCount;
        }


        public void EmptyCart(String id)
        {
            var cartItems = storeDB.Carts.Where(
                cart => cart.CartId == id);

            foreach (var cartItem in cartItems)
            {
                storeDB.Carts.Remove(cartItem);
            }
            // Save changes
            storeDB.SaveChanges();
        }


        public List<Cart> GetCartItems()
        {
            return storeDB.Carts.Where(
                cart => cart.CartId == ShoppingCartId).ToList();
        }
        public int GetCount()
        {
            // Get the count of each item in the cart and sum them up
            int? count = (from cartItems in storeDB.Carts
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.Count).Sum();
            // Return 0 if all entries are null
            return count ?? 0;
        }

        public double GetTax()
        {
            // Get the count of each item in the cart and sum them up
            double? Tax = (from cartItems in storeDB.Carts
                           where cartItems.CartId == ShoppingCartId
                           select (int?)cartItems.Count *
                           cartItems.Product.productPrice).Sum() / 10;

            // Return 0 if all entries are null
            return Tax ?? 0;
        }

        public double GetProductSubTotal(int id)
        {
            //
            if ((from cart in storeDB.Carts
                 where cart.RecordId == id
                 select cart.CartId).Count() != 0)
            {
                var cartItems = storeDB.Carts.Single(
                    cart => cart.CartId == ShoppingCartId
                    && cart.RecordId == id);
                cartItems.ProductSubTotal = (from cartItem in storeDB.Carts
                                             where cartItem.RecordId == id
                                             select (double)cartItems.Product.productPrice).First() * (from cartItem in storeDB.Carts
                                                                                                       where cartItems.RecordId == id
                                                                                                       select (int)cartItems.Count).First();
                // Save changes
                storeDB.SaveChanges();
                return cartItems.ProductSubTotal;
            }
            return 0;
        }

        public double GetTotal()
        {
            // Multiply product price by count of that product to get 
            // the current price for each of those albums in the cart
            // sum all album price totals to get the cart total
            double? total = (from cartItems in storeDB.Carts
                             where cartItems.CartId == ShoppingCartId
                             select (int?)cartItems.Count *
                             cartItems.Product.productPrice).Sum();

            return total ?? (double)0;
        }



        public Guid CreateOrder(Order order)
        {
            double totalPrice = 0;

            var cartItems = GetCartItems();
            // Iterate over the items in the cart, 
            // adding the order details for each

            var loginUserName = Convert.ToString(HttpContext.Current.Session["LoginUser"]);
            long userId = (from r in storeDB.register where r.userName.Equals(loginUserName) select r.userId).FirstOrDefault();



            order = new Order
            {
                OrderId = Guid.NewGuid(),
                totalPrice = totalPrice,
                OrderDate = DateTime.Now,
                OrderStatus = "Pending",
                reasons = ""
            };


            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    OrderDetailId = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    OrderId = order.OrderId,
                    UnitPrice = item.Product.productPrice,
                    Quantity = item.Count,
                    productSize = item.ProductSize,
                    TotalCost = Convert.ToDouble(string.Format("{0:0.00}", item.Product.productPrice * item.Count)),
                    ifPackaged = false
                };

                // Set the order total of the shopping cart
                totalPrice += Convert.ToDouble(string.Format("{0:0.00}", item.Product.productPrice * item.Count));

                storeDB.OrderDetails.Add(orderDetail);

            }


            order.userId = userId;
            order.userName = loginUserName;
            order.totalPrice = totalPrice;

            storeDB.Orders.Add(order);



            // Save the order
            storeDB.SaveChanges();


            // Return the OrderId as the confirmation number
            return order.OrderId;
        }



        // We're using HttpContextBase to allow access to cookies.
        public string GetCartId(HttpContextBase context)
        {
            if (context.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[CartSessionKey] =
                        context.User.Identity.Name;
                }
                else
                {
                    // Generate a new random GUID using System.Guid class
                    Guid tempCartId = Guid.NewGuid();
                    // Send tempCartId back to client as a cookie
                    context.Session[CartSessionKey] = tempCartId.ToString();
                }
            }
            return context.Session[CartSessionKey].ToString();
        }

        // When a user has logged in, migrate their shopping cart to
        // be associated with their username
        public void MigrateCart(string userName)
        {
            var shoppingCart = storeDB.Carts.Where(
                c => c.CartId == ShoppingCartId);

            foreach (Cart item in shoppingCart)
            {
                item.CartId = userName;
            }
            storeDB.SaveChanges();
        }
    }
}