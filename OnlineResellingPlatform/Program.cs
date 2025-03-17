using System;
using System.Collections.Generic;

// Interfaces

public interface ISellerAction
{
    void SExecute();
}

public interface IBuyerAction
{
    void BExecute();
}

public interface IAdminAction
{
    void AExecute();
}

public interface IOrderAction
{
    void Execute();
}

public interface ISearchAction
{
    void Search(List<Product> products);
}



// Base classes
public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; } // "1" for Seller, "2" for Buyer
    public string SubscriptionType { get; set; } // "Basic", "Premium", or null
    public string ContactNumber { get; set; } // Seller's contact number

    public User(string username, string password, string role, string contactNumber = null)
    {
        Username = username;
        Password = password;
        Role = role;
        SubscriptionType = null; // Default to no subscription
        ContactNumber = contactNumber; // Seller's contact number
    }
}

//user authentication
public class AuthenticateUser
{
    private readonly List<User> _users;

    public AuthenticateUser(List<User> users)
    {
        this._users = users;
    }

    public User Login(string username, string password, string role)
    {
        foreach (var user in _users)
        {
            if (user.Username == username && user.Password == password && user.Role == role)
            {
                return user;
            }
        }
        return null;
    }
}


public class Product
{
    private static int _nextId = 1;
    public int Id { get; private set; }
    public string Name { get; set; }
    public string Model { get; set; }
    public string Category { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal DiscountedPrice { get; set; }
    public string Description { get; set; }
    public string Owner { get; set; }
    public List<string> Reviews { get; set; } = new List<string>();
    public double Rating { get; set; } = 0;
    public int Quantity { get; set; } // Quantity of the product
    public bool IsSoldOut { get; set; } // Indicates if the product is sold out

    public Product(string name, string model, string category, decimal originalPrice, decimal discountedPrice, string description, string owner, int quantity)
    {
        this.Id = _nextId++;
        this.Name = name;
        this.Model = model;
        this.Category = category;
        this.OriginalPrice = originalPrice;
        this.DiscountedPrice = discountedPrice;
        this.Description = description;
        this.Owner = owner;
        this.Quantity = quantity;
        this.IsSoldOut = quantity <= 0; // Mark as sold out if quantity is zero or less
    }
}

// Order Class
public class Order
{
    private static int _nextId = 1;
    public int OrderId { get; private set; }
    public string BuyerUsername { get; set; }
    public int ProductId { get; set; }
    public string Status { get; set; } // "Placed", "Shipped", "Delivered", "Cancelled"
    public DateTime OrderDate { get; set; }

    public Order(string buyerUsername, int productId)
    {
        this.OrderId = _nextId++;
        this.BuyerUsername = buyerUsername;
        this.ProductId = productId;
        this.Status = "Placed";
        this.OrderDate = DateTime.Now;
    }
}

// Seller Feedback on Software
public class SellerFeedback
{
    public string SellerUsername { get; set; }
    public string Feedback { get; set; }
    public int Rating { get; set; } // Rating out of 5

    public SellerFeedback(string sellerUsername, string feedback, int rating)
    {
        SellerUsername = sellerUsername;
        Feedback = feedback;
        Rating = rating;
    }
}

// Buyer Feedback on Products
public class BuyerFeedback
{
    public string BuyerUsername { get; set; }
    public int ProductId { get; set; }
    public string Feedback { get; set; }
    public int Rating { get; set; } // Rating out of 5

    public BuyerFeedback(string buyerUsername, int productId, string feedback, int rating)
    {
        BuyerUsername = buyerUsername;
        ProductId = productId;
        Feedback = feedback;
        Rating = rating;
    }
}




// Payment Processing
public class PaymentMethod
{
    protected decimal balance = 1500; // Default balance
    protected decimal minBalance = 300; // Minimum required balance

    public virtual void ProcessPayment(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Invalid Amount");
            return;
        }

        decimal tax = amount * 0.10m; // Default tax rate of 10%
        decimal totalAmount = amount + tax;

        if (totalAmount > balance)
        {
            Console.WriteLine("Insufficient Balance!");
            return;
        }

        Console.WriteLine($"Tax Amount: {tax}");
        Console.WriteLine($"Amount inclusive of Taxes: {totalAmount}");
        balance -= totalAmount;

        if (balance < minBalance)
        {
            Console.WriteLine("Running below Minimum balance.");
            Console.WriteLine($"Please maintain a minimum balance of {minBalance}/-");
        }
    }
}

public class GPay : PaymentMethod
{
    public override void ProcessPayment(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Invalid Amount");
            return;
        }

        decimal tax = amount * 0.05m; // GPay has a lower tax rate of 5%
        decimal totalAmount = amount + tax;

        if (totalAmount > balance)
        {
            Console.WriteLine("Insufficient Balance!");
            return;
        }

        Console.WriteLine($"Tax Amount: {tax}");
        Console.WriteLine($"Amount inclusive of Taxes: {totalAmount}");
        balance -= totalAmount;

        if (balance < minBalance)
        {
            Console.WriteLine("Running below Minimum balance.");
            Console.WriteLine($"Please maintain a minimum balance of {minBalance}/-");
        }

        Console.WriteLine($"Paid rs {amount} via GPay");
    }
}

public class CreditCard : PaymentMethod
{
    public override void ProcessPayment(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Invalid Amount");
            return;
        }

        decimal tax = amount * 0.15m; // CreditCard has a higher tax rate of 15%
        decimal totalAmount = amount + tax;

        if (totalAmount > balance)
        {
            Console.WriteLine("Insufficient Balance!");
            return;
        }

        Console.Write("Card Number: ");
        Console.ReadLine();

        Console.WriteLine($"Tax Amount: {tax}");
        Console.WriteLine($"Amount inclusive of Taxes: {totalAmount}");
        balance -= totalAmount;

        if (balance < minBalance)
        {
            Console.WriteLine("Running below Minimum balance.");
            Console.WriteLine($"Please maintain a minimum balance of {minBalance}/-");
        }

        Console.WriteLine($"Paid rs {amount} via Credit Card");
    }
}



// Subscription Service
public class SubscriptionService
{
    private readonly PaymentMethod _paymentMethod;
    private readonly PaymentMethodSelector _paymentMethodSelector;

    public SubscriptionService(PaymentMethod paymentMethod, PaymentMethodSelector paymentMethodSelector)
    {
        this._paymentMethod = paymentMethod;
        this._paymentMethodSelector = paymentMethodSelector;
    }

    public void ProcessSubscription(User user, string planChoice)
    {
        if (planChoice != "1" && planChoice != "2")
        {
            Console.WriteLine("Invalid choice!");
            return;
        }

        decimal amount = planChoice == "1" ? 500 : 1000;
        user.SubscriptionType = planChoice == "1" ? "Basic" : "Premium";

        Console.WriteLine($"\nAmount to Pay: rs {amount}");

        // Delegate payment method selection to PaymentMethodSelector
        PaymentMethod selectedPaymentMethod = _paymentMethodSelector.ChoosePaymentMethod();

        if (selectedPaymentMethod != null)
        {
            selectedPaymentMethod.ProcessPayment(amount); // Process payment using the selected method
        }
        else
        {
            Console.WriteLine("Invalid payment method choice!");
            return;
        }

        Console.WriteLine($"\nSubscription Activated: {user.SubscriptionType} Account");
    }
}

public class PaymentMethodSelector
{
    public PaymentMethod ChoosePaymentMethod()
    {
        Console.WriteLine("Choose Payment Method:");
        Console.WriteLine("1. GPay");
        Console.WriteLine("2. Credit Card");
        Console.Write("Enter choice: ");
        string choice = Console.ReadLine();

        return choice switch
        {
            "1" => new GPay(),
            "2" => new CreditCard(),
            _ => throw new InvalidOperationException("Invalid payment method choice!")
        };
    }
}

// Feedback Service
public class BuyerFeedbackService
{
    private readonly List<BuyerFeedback> _buyerFeedbacks;

    public BuyerFeedbackService(List<BuyerFeedback> buyerFeedbacks)
    {
        this._buyerFeedbacks = buyerFeedbacks;
    }

    public void AddFeedback(string buyerUsername, int productId, string feedback, int rating)
    {
        _buyerFeedbacks.Add(new BuyerFeedback(buyerUsername, productId, feedback, rating));
    }

    public List<int> GetRatings(string buyerUsername)
    {
        List<int> ratings = new List<int>();
        foreach (var feedback in _buyerFeedbacks)
        {
            if (feedback.BuyerUsername == buyerUsername)
            {
                ratings.Add(feedback.Rating);
            }
        }
        return ratings;
    }
}


public class SellerFeedbackService
{
    private readonly List<SellerFeedback> _sellerFeedbacks;

    public SellerFeedbackService(List<SellerFeedback> sellerFeedbacks)
    {
        this._sellerFeedbacks = sellerFeedbacks;
    }

    public void AddFeedback(string sellerUsername, string feedback, int rating)
    {
        _sellerFeedbacks.Add(new SellerFeedback(sellerUsername, feedback, rating));
    }

    public List<int> GetRatings(string sellerUsername)
    {
        List<int> ratings = new List<int>();
        foreach (var feedback in _sellerFeedbacks)
        {
            if (feedback.SellerUsername == sellerUsername)
            {
                ratings.Add(feedback.Rating);
            }
        }
        return ratings;
    }
}



// OCP - Open for extension, Closed for modification
public sealed class SearchByName : ISearchAction
{
    public void Search(List<Product> products)
    {
        Console.Write("Enter product name to search: ");
        string name = Console.ReadLine();
        bool found = false;

        Console.WriteLine("\nSearch Results:");
        foreach (var product in products)
        {
            if (product.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Console.WriteLine($"ID: {product.Id}, Name: {product.Name}, Price: rs {product.DiscountedPrice}, Model: {product.Model}, Quantity: {product.Quantity}");
                found = true;
            }
        }

        if (!found)
        {
            Console.WriteLine("No products found with the given name.");
        }
    }
}

public sealed class SearchByPrice : ISearchAction
{
    public void Search(List<Product> products)
    {
        Console.Write("Enter maximum price to search: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal maxPrice))
        {
            bool found = false;

            Console.WriteLine("\nSearch Results:");
            foreach (var product in products)
            {
                if (product.DiscountedPrice <= maxPrice)
                {
                    Console.WriteLine($"ID: {product.Id}, Name: {product.Name}, Price: rs {product.DiscountedPrice}, Model: {product.Model}, Quantity: {product.Quantity}");
                    found = true;
                }
            }

            if (!found)
            {
                Console.WriteLine("No products found within the given price range.");
            }
        }
        else
        {
            Console.WriteLine("Invalid price input!");
        }
    }
}

public sealed class SearchByModel : ISearchAction
{
    public void Search(List<Product> products)
    {
        Console.Write("Enter product model to search: ");
        string model = Console.ReadLine();
        bool found = false;

        Console.WriteLine("\nSearch Results:");
        foreach (var product in products)
        {
            if (product.Model.IndexOf(model, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Console.WriteLine($"ID: {product.Id}, Name: {product.Name}, Price: rs {product.DiscountedPrice}, Model: {product.Model}, Quantity: {product.Quantity}");
                found = true;
            }
        }
        if (!found)
        {
            Console.WriteLine("No products found with the given model.");
        }
    }
}

public sealed class SearchByCategory : ISearchAction
{
    public void Search(List<Product> products)
    {
        Console.Write("Enter product category to search: ");
        string category = Console.ReadLine();
        bool found = false;

        Console.WriteLine("\nSearch Results:");
        foreach (var product in products)
        {
            if (product.Category.IndexOf(category, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Console.WriteLine($"ID: {product.Id}, Name: {product.Name}, Price: rs {product.DiscountedPrice}, Category: {product.Category}, Quantity: {product.Quantity}");
                found = true;
            }
        }

        if (!found)
        {
            Console.WriteLine("No products found in the given category.");
        }
    }
}


public class SearchProduct : IBuyerAction
{
    private readonly List<Product> products;

    public SearchProduct(List<Product> products)
    {
        this.products = products;
    }

    public void BExecute()
    {
        while (true)
        {
            Console.WriteLine("\nSearch Products");
            Console.WriteLine("1. Search by Name\n2. Search by Price\n3. Search by Model\n4. Search by Category\n5. Back to Menu");
            Console.Write("Enter choice: ");
            string choice = Console.ReadLine();

            if (choice == "5") // Back to Menu
            {
                break;
            }

            switch (choice)
            {
                case "1":
                    ISearchAction obj1 = new SearchByName();
                    obj1.Search(products);
                    break;
                case "2":
                    ISearchAction obj2 = new SearchByPrice();
                    obj2.Search(products);
                    break;
                case "3":
                    ISearchAction obj3 = new SearchByModel();
                    obj3.Search(products);
                    break;
                case "4":
                    ISearchAction obj4 = new SearchByCategory();
                    obj4.Search(products);
                    break;
                default:
                    Console.WriteLine("Invalid choice!");
                    break;
            }
        }
    }
}



// ORDER CLASSES

public class PlaceOrder : IOrderAction
{
    private readonly List<Order> _orders;
    private readonly List<Product> _products;
    private readonly InventoryService _inventoryService;

    public string BuyerUsername { get; } // Expose BuyerUsername as a public property

    public PlaceOrder(List<Order> orders, List<Product> products, string buyerUsername, InventoryService inventoryService)
    {
        this._orders = orders;
        this._products = products;
        this.BuyerUsername = buyerUsername; // Set the BuyerUsername
        this._inventoryService = inventoryService;
    }

    public void Execute()
    {
        Console.Write("Enter Product ID to Order: ");
        int productId = Convert.ToInt32(Console.ReadLine());
        Product product = _products.FirstOrDefault(p => p.Id == productId);

        if (product != null)
        {
            if (product.Quantity > 0)
            {
                _orders.Add(new Order(BuyerUsername, productId));
                _inventoryService.UpdateStock(product, -1);
                Console.WriteLine("Order placed successfully!");
            }
            else
            {
                Console.WriteLine("Product is sold out!");
            }
        }
        else
        {
            Console.WriteLine("Invalid Product ID!");
        }
    }
}


public class CancelOrder : IOrderAction
{
    private readonly List<Order> _orders;
    private readonly List<Product> _products;
    private readonly string _buyerUsername;
    private readonly InventoryService _inventoryService;

    public CancelOrder(List<Order> orders, List<Product> products, string buyerUsername, InventoryService inventoryService)
    {
        this._orders = orders;
        this._products = products;
        this._buyerUsername = buyerUsername;
        this._inventoryService = inventoryService;
    }

    public void Execute()
    {
        Console.Write("Enter Order ID to Cancel: ");
        int orderId = Convert.ToInt32(Console.ReadLine());
        Order order = _orders.FirstOrDefault(o => o.OrderId == orderId && o.BuyerUsername == _buyerUsername);

        if (order != null)
        {
            order.Status = "Cancelled";
            Product product = _products.FirstOrDefault(p => p.Id == order.ProductId);

            if (product != null)
            {
                _inventoryService.UpdateStock(product, 1);
                Console.WriteLine("Order cancelled successfully!");
            }
        }
        else
        {
            Console.WriteLine("Invalid Order ID or you do not have permission to cancel this order!");
        }
    }
}

public class TrackOrder : IBuyerAction
{
    private readonly List<Order> _orders;
    private readonly string _buyerUsername;

    public TrackOrder(List<Order> orders, string buyerUsername)
    {
        this._orders = orders;
        this._buyerUsername = buyerUsername;
    }

    public void BExecute()
    {
        Console.Write("Enter Order ID to Track: ");
        int orderId = Convert.ToInt32(Console.ReadLine());
        Order foundOrder = null;

        // Iterate through the list of orders to find the matching order
        foreach (var order in _orders)
        {
            if (order.OrderId == orderId && order.BuyerUsername == this._buyerUsername)
            {
                foundOrder = order;
                break; // Exit the loop once the order is found
            }
        }

        if (foundOrder != null)
        {
            Console.WriteLine($"Order ID: {foundOrder.OrderId}, Status: {foundOrder.Status}, Order Date: {foundOrder.OrderDate}");
        }
        else
        {
            Console.WriteLine("Invalid Order ID or you do not have permission to track this order!");
        }
    }
}

// Inventory Management
public class InventoryService
{
    public void UpdateStock(Product product, int quantityChange)
    {
        product.Quantity += quantityChange;
        product.IsSoldOut = product.Quantity <= 0;
    }
}



// Register User
public class RegisterUser : IAdminAction
{
    private readonly List<User> _users;

    public RegisterUser(List<User> users)
    {
        this._users = users;
    }

    public void AExecute()
    {
        Console.Write("Enter Username: ");
        string username = Console.ReadLine();
        Console.Write("Enter Password: ");
        string password = Console.ReadLine();
        Console.Write("Enter Role (1 for Seller, 2 for Buyer): ");
        string role = Console.ReadLine();
        string contactNumber = null;

        if (role == "1")
        {
            Console.Write("Enter Contact Number: ");
            contactNumber = Console.ReadLine();
        }

        _users.Add(new User(username, password, role, contactNumber));
        Console.WriteLine("User registered successfully!");
    }
}

// Admin Menu
public class AdminMenu : IAdminAction
{
    private readonly List<User> _users;
    private readonly List<Product> _products;
    private readonly List<SellerFeedback> _sellerFeedbacks;
    private readonly List<BuyerFeedback> _buyerFeedbacks;

    public AdminMenu(List<User> users, List<Product> products, List<SellerFeedback> sellerFeedbacks, List<BuyerFeedback> buyerFeedbacks)
    {
        _users = users;
        _products = products;
        _sellerFeedbacks = sellerFeedbacks;
        _buyerFeedbacks = buyerFeedbacks;
    }

    public void AExecute()
    {
        while (true)
        {
            Console.WriteLine("\nAdmin Menu");
            Console.WriteLine("1. View Users\n2. View Products\n3. View Seller Feedbacks\n4. View Buyer Feedbbacks\n5. Exit\n");
            Console.Write("Enter choice: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                foreach (var user in _users)
                {
                    Console.WriteLine($"Username: {user.Username}, Role: {user.Role}, Subscription: {user.SubscriptionType}");
                }
            }
            else if (choice == "2")
            {
                foreach (var product in _products)
                {
                    Console.WriteLine($"ID: {product.Id}, Name: {product.Name}, Quantity: {product.Quantity}, Sold Out: {product.IsSoldOut}");
                }
            }
            else if (choice == "3")
            {
                foreach (var feedback in _sellerFeedbacks)
                {
                    Console.WriteLine($"Seller: {feedback.SellerUsername}, Feedback: {feedback.Feedback}, Rating: {feedback.Rating}");
                }
            }
            else if (choice == "4")
            {
                foreach (var feedback in _buyerFeedbacks)
                {
                    Console.WriteLine($"Buyer: {feedback.BuyerUsername}, Feedback: {feedback.Feedback}, Rating: {feedback.Rating}");
                }
            }
            else if (choice == "5")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid choice!");
            }
        }
    }
}


// View products
public class ViewProducts : IBuyerAction
{
    private readonly List<Product> _products;

    public ViewProducts(List<Product> products)
    {
        this._products = products;
    }

    public void BExecute()
    {
        Console.WriteLine("\nAvailable Products:");
        foreach (var product in _products)
        {
            Console.WriteLine($"ID: {product.Id}, Name: {product.Name}, Price: Rs.{product.DiscountedPrice}, Quantity: {product.Quantity}, Sold Out: {product.IsSoldOut}");
        }
    }

}

// Add Product
public class AddProduct : ISellerAction
{
    private readonly List<Product> _products;

    public string SellerUsername { get; } // Expose SellerUsername as a public property

    public AddProduct(List<Product> products, string sellerUsername)
    {
        this._products = products;
        this.SellerUsername = sellerUsername; // Set the SellerUsername
    }

    public void SExecute()
    {
        Console.Write("Enter Product Name: ");
        string name = Console.ReadLine();
        Console.Write("Enter Model: ");
        string model = Console.ReadLine();
        Console.Write("Enter Category: ");
        string category = Console.ReadLine();
        Console.Write("Enter Original Price (INR): ");
        decimal originalPrice = Convert.ToDecimal(Console.ReadLine());
        Console.Write("Enter Discounted Price (INR): ");
        decimal discountedPrice = Convert.ToDecimal(Console.ReadLine());
        Console.Write("Enter Description: ");
        string description = Console.ReadLine();
        Console.Write("Enter Quantity: ");
        int quantity = Convert.ToInt32(Console.ReadLine());

        _products.Add(new Product(name, model, category, originalPrice, discountedPrice, description, SellerUsername, quantity));
        Console.WriteLine("Product added successfully!");
    }
}


// Update Product
public class UpdateProduct : ISellerAction
{
    private readonly List<Product> _products;

    public UpdateProduct(List<Product> products)
    {
        this._products = products;
    }

    public void SExecute()
    {
        Console.Write("Enter Product ID to Update: ");
        int productId = Convert.ToInt32(Console.ReadLine());
        Product product = _products.FirstOrDefault(p => p.Id == productId);

        if (product != null)
        {
            Console.Write("Enter New Product Name: ");
            product.Name = Console.ReadLine();
            Console.Write("Enter New Model: ");
            product.Model = Console.ReadLine();
            Console.Write("Enter New Category: ");
            product.Category = Console.ReadLine();
            Console.Write("Enter New Discounted Price: ");
            product.DiscountedPrice = Convert.ToDecimal(Console.ReadLine());
            Console.Write("Enter New Quantity: ");
            product.Quantity = Convert.ToInt32(Console.ReadLine());
            product.IsSoldOut = product.Quantity <= 0; // Update sold out status
            Console.WriteLine("Product updated successfully!");
        }
        else
        {
            Console.WriteLine("Product not found!");
        }
    }
}

// Delete Product
public class DeleteProduct : ISellerAction
{
    private readonly List<Product> _products;

    public DeleteProduct(List<Product> products)
    {
        this._products = products;
    }

    public void SExecute()
    {
        Console.Write("Enter Product ID to Delete: ");
        int productId = Convert.ToInt32(Console.ReadLine());
        Product product = _products.FirstOrDefault(p => p.Id == productId);

        if (product != null)
        {
            _products.Remove(product);
            Console.WriteLine("Product deleted successfully!");
        }
        else
        {
            Console.WriteLine("Product not found!");
        }
    }
}

// User Menu
public class UserMenu
{
    private readonly User _user;
    private readonly SellerMenu _sellerMenu;
    private readonly BuyerMenu _buyerMenu;

    public UserMenu(User user, SellerMenu sellerMenu, BuyerMenu buyerMenu)
    {
        this._user = user;
        this._sellerMenu = sellerMenu;
        this._buyerMenu = buyerMenu;
    }

    public void Execute()
    {
        if (_user.Role == "1") // Seller
        {
            _sellerMenu.Execute();
        }
        else if (_user.Role == "2") // Buyer
        {
            _buyerMenu.Execute();
        }
    }
}

public class SellerMenu
{
    private readonly ViewProducts _viewProducts;
    private readonly AddProduct _addProduct;
    private readonly UpdateProduct _updateProduct;
    private readonly DeleteProduct _deleteProduct;
    private readonly SellerFeedbackService _sellerFeedbackService;
    private readonly SubscriptionService _subscriptionService;
    private readonly User _seller;

    public SellerMenu(ViewProducts viewProducts, AddProduct addProduct, UpdateProduct updateProduct, DeleteProduct deleteProduct, SellerFeedbackService sellerFeedbackService, SubscriptionService subscriptionService, User seller)
    {
        this._viewProducts = viewProducts;
        this._addProduct = addProduct;
        this._updateProduct = updateProduct;
        this._deleteProduct = deleteProduct;
        this._sellerFeedbackService = sellerFeedbackService;
        this._subscriptionService = subscriptionService;
        this._seller = seller;
    }

    public void Execute()
    {
        while (true)
        {
            Console.WriteLine("\nSeller Menu");
            Console.WriteLine("1. View Products\n2. Add Product\n3. Update Product\n4. Delete Product\n5. Leave Feedback\n6. Subscribe\n7. Exit");
            Console.Write("Enter choice: ");
            string choice = Console.ReadLine();

            if (choice == "1")
                _viewProducts.BExecute();
            else if (choice == "2")
                _addProduct.SExecute();
            else if (choice == "3")
                _updateProduct.SExecute();
            else if (choice == "4")
                _deleteProduct.SExecute();
            else if (choice == "5")
            {
                Console.Write("Enter Feedback: ");
                string feedback = Console.ReadLine();
                Console.Write("Enter Rating (1-5): ");
                int rating = Convert.ToInt32(Console.ReadLine());
                _sellerFeedbackService.AddFeedback(_addProduct.SellerUsername, feedback, rating);
            }
            else if (choice == "6")
            {
                Console.Write("Choose Subscription (1: Basic, 2: Premium): ");
                string planChoice = Console.ReadLine();
                _subscriptionService.ProcessSubscription(_seller, planChoice);
            }
            else if (choice == "7")
                break;
            else
                Console.WriteLine("Invalid choice!");
        }
    }
}

// Buyer Menu
public class BuyerMenu
{
    private readonly PlaceOrder _placeOrder;
    private readonly CancelOrder _cancelOrder;
    private readonly TrackOrder _trackOrder;
    private readonly BuyerFeedbackService _buyerFeedbackService;
    private readonly ViewProducts _viewProducts;
    private readonly SearchProduct _searchProduct;
    private readonly SubscriptionService _subscriptionService;
    private readonly User _buyer;

    public BuyerMenu(PlaceOrder placeOrder, CancelOrder cancelOrder, TrackOrder trackOrder, BuyerFeedbackService buyerFeedbackService, ViewProducts viewProducts, SearchProduct searchProduct, SubscriptionService subscriptionService, User buyer)
    {
        this._placeOrder = placeOrder;
        this._cancelOrder = cancelOrder;
        this._trackOrder = trackOrder;
        this._buyerFeedbackService = buyerFeedbackService;
        this._viewProducts = viewProducts;
        this._searchProduct = searchProduct;
        this._subscriptionService = subscriptionService;
        this._buyer = buyer;
    }

    public void Execute()
    {
        while (true)
        {
            Console.WriteLine("\nBuyer Menu");
            Console.WriteLine("1. View Products\n2. Search Products\n3. Place Order\n4. Cancel Order\n5. Track Order\n6. Leave Feedback\n7. Subscribe\n8. Exit");
            Console.Write("Enter choice: ");
            string choice = Console.ReadLine();

            if (choice == "1")
                _viewProducts.BExecute();
            else if (choice == "2")
                _searchProduct.BExecute();
            else if (choice == "3")
                _placeOrder.Execute();
            else if (choice == "4")
                _cancelOrder.Execute();
            else if (choice == "5")
                _trackOrder.BExecute();
            else if (choice == "6")
            {
                Console.Write("Enter Product ID: ");
                int productId = Convert.ToInt32(Console.ReadLine());
                Console.Write("Enter Feedback: ");
                string feedback = Console.ReadLine();
                Console.Write("Enter Rating (1-5): ");
                int rating = Convert.ToInt32(Console.ReadLine());
                _buyerFeedbackService.AddFeedback(_placeOrder.BuyerUsername, productId, feedback, rating);
            }
            else if (choice == "7")
            {
                Console.Write("Choose Subscription (1: Basic, 2: Premium): ");
                string planChoice = Console.ReadLine();
                _subscriptionService.ProcessSubscription(_buyer, planChoice);
            }
            else if (choice == "8")
                break;
            else
                Console.WriteLine("Invalid choice!");
        }
    }
}



// Main Program
public class Program
{
    static List<Order> orders = new List<Order>();
    static List<User> users = new List<User>();
    static List<Product> products = new List<Product>();
    static List<SellerFeedback> sellerFeedbacks = new List<SellerFeedback>();
    static List<BuyerFeedback> buyerFeedbacks = new List<BuyerFeedback>();

    public static void Main()
    {
        string adminUsername = "admin";
        string adminPassword = "admin123";

        var inventoryService = new InventoryService();
        var paymentMethodSelector = new PaymentMethodSelector(); // Create PaymentMethodSelector
        var subscriptionService = new SubscriptionService(null, paymentMethodSelector); // Pass PaymentMethodSelector
        var buyerFeedbackService = new BuyerFeedbackService(buyerFeedbacks);
        var sellerFeedbackService = new SellerFeedbackService(sellerFeedbacks);
        var authService = new AuthenticateUser(users);

        while (true)
        {
            Console.WriteLine("\nWelcome to the Online Reselling Platform");
            Console.WriteLine("1. Register\n2. Login\n3. Exit\n");
            Console.Write("Enter choice: ");
            string choice = Console.ReadLine();

            if (choice == "1")
                new RegisterUser(users).AExecute();
            else if (choice == "2")
            {
                Console.Write("\nAre you an 1.Seller, 2.Buyer, 3.Admin?: ");
                string userType = Console.ReadLine();

                Console.Write("Enter Username: ");
                string username = Console.ReadLine();
                Console.Write("Enter Password: ");
                string password = Console.ReadLine();

                if (userType == "3") // Admin
                {
                    if (username == adminUsername && password == adminPassword)
                        new AdminMenu(users, products, sellerFeedbacks, buyerFeedbacks).AExecute();
                    else
                        Console.WriteLine("Invalid Admin credentials!");
                }
                else if (userType == "1" || userType == "2") // Seller (1) or Buyer (2)
                {
                    string mappedRole = userType == "1" ? "1" : "2";
                    User user = authService.Login(username, password, mappedRole);

                    if (user != null)
                    {
                        if (user.Role == "1") // Seller
                        {
                            var sellerMenu = new SellerMenu(
                                new ViewProducts(products),
                                new AddProduct(products, user.Username),
                                new UpdateProduct(products),
                                new DeleteProduct(products),
                                sellerFeedbackService,
                                subscriptionService,
                                user // Pass the logged-in seller
                            );

                            var userMenu = new UserMenu(user, sellerMenu, null); // No buyer menu for seller
                            userMenu.Execute();
                        }
                        else if (user.Role == "2") // Buyer
                        {
                            var buyerMenu = new BuyerMenu(
                                new PlaceOrder(orders, products, user.Username, inventoryService),
                                new CancelOrder(orders, products, user.Username, inventoryService),
                                new TrackOrder(orders, user.Username),
                                buyerFeedbackService,
                                new ViewProducts(products),
                                new SearchProduct(products),
                                subscriptionService,
                                user // Pass the logged-in buyer
                            );

                            var userMenu = new UserMenu(user, null, buyerMenu); // No seller menu for buyer
                            userMenu.Execute();
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nInvalid credentials or role! Try again.");
                    }
                }
                else
                {
                    Console.WriteLine("\nInvalid choice! Try again.");
                }
            }
            else if (choice == "3")
                return;
            else
                Console.WriteLine("\nInvalid choice! Try again.");
        }
    }
}
